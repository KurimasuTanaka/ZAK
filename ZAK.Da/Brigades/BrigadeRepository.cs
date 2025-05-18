using System;
using Microsoft.EntityFrameworkCore;
using ZAK.DA;
using ZAK.Db;

namespace ZAK.Da.Brigades;

public class BrigadeRepository : IBrigadeRepository
{
    private readonly IDbContextFactory<ZakDbContext> _dbContextFactory;

    public BrigadeRepository(IDbContextFactory<ZakDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task CreateAsync(Brigade entity)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            context.brigades.Add(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            var entity = context.brigades.Find(id);
            if (entity != null)
            {
                context.brigades.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }

    public async Task<IEnumerable<Brigade>> GetAllAsync()
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            return await context.brigades
                .Include(b => b.scheduledApplications)
                .Select(b => new Brigade(b))
                .ToListAsync();
        }
    }

    public async Task<IEnumerable<Brigade>> GetAllWithScheduledApplicationInfoAsync()
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            return await context.brigades
                .Include(b => b.scheduledApplications)
                .ThenInclude(sa => sa.application).ThenInclude(a => a.address).ThenInclude(a => a.coordinates)
                .Select(b => new Brigade(b))
                .ToListAsync();
        }
    }

    public async Task<Brigade> GetByIdAsync(int id)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            var brigade = await context.brigades.FindAsync(id);

            if (brigade is not null)
            {
                return new Brigade(brigade);
            }
            else
            {
                throw new Exception($"Brigade with id {id} not found");
            }
        }
    }

    public Task UpdateAsync(Brigade entity)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            context.brigades.Update(entity);
            return context.SaveChangesAsync();
        }
    }
}
