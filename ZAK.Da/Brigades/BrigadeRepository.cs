using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ZAK.DA;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class BrigadeRepository : IBrigadeRepository
{
    private readonly IDbContextFactory<ZakDbContext> _dbContextFactory;
    private readonly ILogger<BrigadeRepository> _logger;
    private readonly IMemoryCache _cache;

    string brigadesCacheKey = "Brigades";
    string brigadesWithAppsCacheKey = "BrigadesWithApps";

    public BrigadeRepository(IDbContextFactory<ZakDbContext> dbContextFactory, ILogger<BrigadeRepository> logger, IMemoryCache cache)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _cache = cache;
    }
    private void DropCache()
    {
        _cache.Remove(brigadesCacheKey);
        _cache.Remove(brigadesWithAppsCacheKey);
    }

    public async Task CreateAsync(Brigade entity)
    {
        _logger.LogInformation("Creating brigade: {@Brigade}", entity);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                context.brigades.Add(entity);

                if (!entity.scheduledApplications.IsNullOrEmpty())
                {
                    List<Application> scheduledApplications = new List<Application>();

                    foreach (var scheduledApplication in entity.scheduledApplications)
                    {
                        if (scheduledApplications.Contains(scheduledApplication.application))
                        {
                            context.Attach(scheduledApplication.application);
                        }
                    }
                    DropCache();
                }
                else
                {
                    _logger.LogWarning("No scheduled applications provided for brigade creation");
                }

                await context.SaveChangesAsync();
            }
            _logger.LogInformation("Brigade created successfully: {@Brigade}", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating brigade: {@Brigade}", entity);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting brigade with id: {Id}", id);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var entity = context.brigades.Find(id);
                if (entity != null)
                {
                    context.brigades.Remove(entity);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Brigade deleted successfully: {Id}", id);
                    DropCache();
                }
                else
                {
                    _logger.LogWarning("Brigade with id {Id} not found for deletion", id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting brigade with id: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Brigade>> GetAllAsync()
    {
        _logger.LogInformation("Getting all brigades");

        if (_cache.TryGetValue(brigadesCacheKey, out IEnumerable<Brigade>? cachedBrigades) && cachedBrigades is not null)
        {
            _logger.LogInformation("Returning cached brigades");
            return cachedBrigades;
        }

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.brigades.AsSplitQuery().AsNoTracking()
                    .Include(b => b.scheduledApplications).ThenInclude(sa => sa.application).ThenInclude(a => a.address).ThenInclude(a => a!.district)
                    .Select(b => new Brigade(b))
                    .ToListAsync();

                _cache.Set(brigadesCacheKey, result, TimeSpan.FromMinutes(5));

                _logger.LogInformation("Retrieved {Count} brigades", result.Count);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all brigades");
            throw;
        }
    }

    public async Task<IEnumerable<Brigade>> GetAllWithScheduledApplicationInfoAsync()
    {
        _logger.LogInformation("Getting all brigades with scheduled application info");

        if (_cache.TryGetValue(brigadesWithAppsCacheKey, out IEnumerable<Brigade>? cachedBrigades) && cachedBrigades is not null)
        {
            _logger.LogInformation("Returning cached brigades");
            return cachedBrigades;
        }

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.brigades.AsSplitQuery().AsNoTracking()
                    .Include(b => b.scheduledApplications)
                    .ThenInclude(sa => sa.application).ThenInclude(a => a.address).ThenInclude(a => a!.coordinates)
                    .Select(b => new Brigade(b))
                    .ToListAsync();

                _cache.Set(brigadesWithAppsCacheKey, result, TimeSpan.FromMinutes(5));
                _logger.LogInformation("Retrieved {Count} brigades with scheduled application info", result.Count);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all brigades with scheduled application info");
            throw;
        }
    }

    public async Task<Brigade> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting brigade by id: {Id}", id);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var brigade = await context.brigades.AsNoTracking().Include(a => a.scheduledApplications).FirstOrDefaultAsync(b => b.id == id);

                if (brigade is not null)
                {
                    _logger.LogInformation("Brigade retrieved: {@Brigade}", brigade);
                    return new Brigade(brigade);
                }
                else
                {
                    _logger.LogWarning("Brigade with id {Id} not found", id);
                    throw new Exception($"Brigade with id {id} not found");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting brigade by id: {Id}", id);
            throw;
        }
    }

    public async Task UpdateAsync(Brigade entity)
    {
        _logger.LogInformation("Updating brigade: {@Brigade}", entity);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                BrigadeModel? brigade = await context.brigades.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application).FirstOrDefaultAsync(b => b.id == entity.id);

                if (brigade == null)
                {
                    _logger.LogWarning("Brigade with id {Id} not found for update", entity.id);
                    throw new Exception($"Brigade with id {entity.id} not found");
                }
                else
                {
                    context.Entry(brigade).CurrentValues.SetValues(entity);

                    if (entity.scheduledApplications != null)
                    {
                        foreach (var scheduledApplication in brigade.scheduledApplications.Except(entity.scheduledApplications, new ScheduledApplicationModelComparer()))
                        {
                            context.Entry(scheduledApplication).State = EntityState.Deleted;
                        }

                        foreach (var scheduledApplication in entity.scheduledApplications.Except(brigade.scheduledApplications, new ScheduledApplicationModelComparer()))
                        {
                            brigade.scheduledApplications.Add(scheduledApplication);
                        }
                    }
                    await context.SaveChangesAsync();
                    DropCache();
                    _logger.LogInformation("Brigade updated successfully: {@Brigade}", entity);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating brigade: {@Brigade}", entity);
            throw;
        }
    }
}

internal class ScheduledApplicationModelComparer : IEqualityComparer<ScheduledApplicationModel>
{
    public bool Equals(ScheduledApplicationModel? x, ScheduledApplicationModel? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;

        return x.applicationId == y.applicationId && x.scheduledTime == y.scheduledTime && x.brigadeId == y.brigadeId;
    }

    public int GetHashCode([DisallowNull] ScheduledApplicationModel obj)
    {
        if (obj is null) throw new ArgumentNullException(nameof(obj));

        return HashCode.Combine(obj.applicationId, obj.scheduledTime, obj.brigadeId);
    }
}