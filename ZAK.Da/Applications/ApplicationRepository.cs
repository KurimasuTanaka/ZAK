using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class ApplicationRepository : IApplicationReporisory
{
    private readonly IDbContextFactory<ZakDbContext> _dbContextFactory;
    private readonly ILogger<ApplicationRepository> _logger;

    public ApplicationRepository(IDbContextFactory<ZakDbContext> dbContextFactory, ILogger<ApplicationRepository> logger)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }


    public async Task CreateAsync(Application entity)
    {
        _logger.LogInformation("Creating application: {@Application}", entity);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                if (entity.address is not null)
                {
                    AddressModel? addressModel = context.addresses.FirstOrDefault(add =>
                        entity.address.streetName == add.streetName && entity.address.building == add.building);

                    if (addressModel is not null) entity.address = addressModel;
                }

                context.applications.Add(entity);
                await context.SaveChangesAsync();
            }
            _logger.LogInformation("Application created successfully: {@Application}", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application: {@Application}", entity);
            throw;
        }
    }

    public async Task CreateRangeAsync(IEnumerable<Application> entities)
    {
        _logger.LogInformation("Creating range of applications. Count: {Count}", entities.Count());

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                List<AddressModel> addresses = context.addresses.ToList();

                foreach (var application in entities)
                {
                    if (application.address is not null)
                    {
                        AddressModel? address = addresses.FirstOrDefault(add =>
                            application.address.streetName == add.streetName && application.address.building == add.building);

                        if (address is not null) application.address = address;
                    }
                }

                context.applications.AddRange(entities);
                await context.SaveChangesAsync();
            }
            _logger.LogInformation("Range of applications created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating range of applications");
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting application with id: {Id}", id);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var entity = context.applications.Find(id);
                if (entity != null)
                {
                    context.applications.Remove(entity);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Application deleted successfully: {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Application with id {Id} not found for deletion", id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting application with id: {Id}", id);
            throw;
        }
    }

    public async Task DeleteRangeAsync(IEnumerable<Application> entities)
    {
        _logger.LogInformation("Deleting range of applications. Count: {Count}", entities.Count());

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var idsToDelete = entities.Select(e => e.id).ToList();
                var applications = context.applications.Where(app => idsToDelete.Contains(app.id)).ToList();
                context.applications.RemoveRange(applications);
                await context.SaveChangesAsync();
                _logger.LogInformation("Range of applications deleted successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting range of applications");
            throw;
        }
    }

    public async Task<IEnumerable<Application>> GetAllAsync()
    {
        _logger.LogInformation("Getting all applications");

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.applications.AsNoTracking()
                    .Include(a => a.address).ThenInclude(a => a.district)
                    .Include(a => a.address.coordinates)
                    .Select(a => new Application(a))
                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} applications", result.Count);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all applications");
            throw;
        }
    }

    public async Task<IEnumerable<Application>> GetAllUpdatedAsync()
    {
        _logger.LogInformation("Getting all updated applications");

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var result = context.applications.AsNoTracking()
                    .Include(a => a.address).ThenInclude(a => a.district)
                    .Include(a => a.address.coordinates)
                    .Select(a => new Application(a))
                    .ToList()
                    .Where(a => a.applicationWasUpdated).ToList();
                _logger.LogInformation("Retrieved {Count} updated applications", result.Count);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all updated applications");
            throw;
        }
    }

    public async Task<IEnumerable<Application>> GetAllWithIgnoringAsync()
    {
        _logger.LogInformation("Getting all applications with ignoring");

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var result = context.applications.AsNoTracking()
                    .Include(a => a.address).ThenInclude(a => a.district)
                    .Include(a => a.address.coordinates)
                    .Select(a => new Application(a)).ToList()
                    .Where(a => !a.ignored)
                    .ToList();
                _logger.LogInformation("Retrieved {Count} applications (not ignored)", result.Count);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all applications with ignoring");
            throw;
        }
    }

    public async Task<Application> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting application by id: {Id}", id);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var application = await context.applications.AsNoTracking()
                    .Include(a => a.address).ThenInclude(a => a.district)
                    .Include(a => a.address.coordinates)
                    .Select(a => new Application(a))
                    .FirstOrDefaultAsync(a => a.id == id);

                if (application is not null)
                {
                    _logger.LogInformation("Application retrieved: {@Application}", application);
                    return application;
                }
                else
                {
                    _logger.LogWarning("Application with id {Id} not found", id);
                    throw new Exception($"Application with id {id} not found");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application by id: {Id}", id);
            throw;
        }
    }

    public async Task UpdateAsync(Application entity)
    {
        _logger.LogInformation("Updating application: {@Application}", entity);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var application = await context.applications
                    .Include(a => a.address)
                    .FirstOrDefaultAsync(a => a.id == entity.id);

                if (application == null)
                {
                    _logger.LogWarning("Application with id {Id} not found for update", entity.id);
                    throw new Exception($"Application with id {entity.id} not found");
                }
                else
                {
                    context.Entry(application).CurrentValues.SetValues(entity);
                    if (entity.address is not null)
                    {
                        if (application.address is null)
                        {
                            application.address = entity.address;
                            context.Entry(application.address).State = EntityState.Added;
                        }
                        else
                        {
                            context.Entry(application.address).CurrentValues.SetValues(entity.address);
                        }
                    }
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Application updated successfully: {@Application}", entity);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application: {@Application}", entity);
            throw;
        }
    }

    public async Task UpdateRangeAsync(IEnumerable<Application> entities)
    {
        _logger.LogInformation("Updating range of applications. Count: {Count}", entities.Count());

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                context.applications.UpdateRange(entities);
                await context.SaveChangesAsync();
                _logger.LogInformation("Range of applications updated successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating range of applications");
            throw;
        }
    }
}
