using System;
using Microsoft.EntityFrameworkCore;
using ZAK.Db;
using ZAK.Db.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace ZAK.DA;

public class CoefficientRepository : ICoefficientRepository
{
    private readonly IDbContextFactory<ZakDbContext> _dbContextFactory;
    private readonly ILogger<CoefficientRepository> _logger;
    private readonly IMemoryCache _cache;

    public CoefficientRepository(IDbContextFactory<ZakDbContext> dbContextFactory, ILogger<CoefficientRepository> logger, IMemoryCache cache)
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    public Task CreateAsync(Coefficient entity)
    {
        _logger.LogWarning("CreateAsync should not be called for CoefficientRepository");
        throw new Exception("Method should not be called");
    }

    public Task DeleteAsync(int id)
    {
        _logger.LogWarning("DeleteAsync should not be called for CoefficientRepository");
        throw new Exception("Method should not be called");
    }

    public async Task<IEnumerable<Coefficient>> GetAllAsync()
    {



        _logger.LogInformation("Getting all coefficients");


        if (_cache.TryGetValue("AllCoefficients", out IEnumerable<Coefficient>? cachedCoefficients) && cachedCoefficients is not null)
        {
            return cachedCoefficients;
        }

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.coefficients.AsNoTracking()
                    .Select(c => new Coefficient(c))
                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} coefficients", result.Count);

                _cache.Set("AllCoefficients", result, TimeSpan.FromMinutes(10));

                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all coefficients");
            throw;
        }
    }

    public async Task<Coefficient> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting coefficient by id: {Id}", id);
        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                CoefficientModel? coefficientModel = await context.coefficients.AsNoTracking().Where(c => c.id == id).FirstOrDefaultAsync();
                if (coefficientModel is null)
                {
                    _logger.LogWarning("Coefficient with id {Id} not found", id);
                    throw new Exception($"Coefficient with id {id} not found");
                }
                _logger.LogInformation("Coefficient retrieved: {@Coefficient}", coefficientModel);
                return new Coefficient(coefficientModel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting coefficient by id: {Id}", id);
            throw;
        }
    }

    public async Task UpdateAsync(Coefficient entity)
    {
        _logger.LogInformation("Updating coefficient: {@Coefficient}", entity);
        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                CoefficientModel? coefficientModel = await context.coefficients.Where(c => c.id == entity.id).FirstOrDefaultAsync();
                if (coefficientModel is not null)
                {
                    coefficientModel.coefficient = entity.coefficient;
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Coefficient updated successfully: {@Coefficient}", entity);

                    _cache.Remove("AllCoefficients");
                }
                else
                {
                    _logger.LogWarning("Coefficient with id {Id} not found for update", entity.id);
                    throw new Exception($"Coefficient with id {entity.id} not found");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating coefficient: {@Coefficient}", entity);
            throw;
        }
    }
}
