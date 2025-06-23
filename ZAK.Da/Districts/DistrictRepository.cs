using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZAK.DA;
using ZAK.Db;

namespace ZAK.DA;

public class DistrictRepository : IDistrictRepository
{
    ILogger<DistrictRepository> _logger;
    IDbContextFactory<ZakDbContext> _dbContextFactory;
    public DistrictRepository(ILogger<DistrictRepository> logger, IDbContextFactory<ZakDbContext> dbContextFactory)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<List<District>> GetAllAsync()
    {
        _logger.LogInformation("Getting all applications");

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.districts
                    .AsNoTracking().Select(d => new District(d))
                    .ToListAsync();
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all applications");
            throw;
        }

    }
}
