using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ZAK.DA;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

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
            var brigade = await context.brigades.Include(a => a.scheduledApplications).FirstOrDefaultAsync(b => b.id == id);

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

    public async Task UpdateAsync(Brigade entity)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            BrigadeModel? brigade = await context.brigades.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application).FirstOrDefaultAsync(b => b.id == entity.id);

            if (brigade == null)
            {
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
            }
            await context.SaveChangesAsync();
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