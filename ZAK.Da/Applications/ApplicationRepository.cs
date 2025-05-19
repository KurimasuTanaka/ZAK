using System;
using Microsoft.EntityFrameworkCore;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class ApplicationRepository : IApplicationReporisory
{
    private readonly IDbContextFactory<ZakDbContext> _dbContextFactory;

    public ApplicationRepository(IDbContextFactory<ZakDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }


    public async Task CreateAsync(Application entity)
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
    }

    public async Task CreateRangeAsync(IEnumerable<Application> entities)
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
    }

    public async Task DeleteAsync(int id)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            var entity = context.applications.Find(id);
            if (entity != null)
            {
                context.applications.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }

    public Task DeleteRangeAsync(IEnumerable<Application> entities)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            var idsToDelete = entities.Select(e => e.id).ToList();
            var applications = context.applications.Where(app => idsToDelete.Contains(app.id)).ToList();
            context.applications.RemoveRange(applications);
            return context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Application>> GetAllAsync()
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            return await context.applications
                .Include(a => a.address).ThenInclude(a => a.district)
                .Include(a => a.address.coordinates)
                .Select(a => new Application(a))
                .ToListAsync();
        }
    }

    public async Task<IEnumerable<Application>> GetAllUpdatedAsync()
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            return await context.applications
                .Include(a => a.address).ThenInclude(a => a.district)
                .Include(a => a.address.coordinates)
                .Select(a => new Application(a))
                .Where(a => a.applicationWasUpdated)
                .ToListAsync();
        }
    }

    public async Task<IEnumerable<Application>> GetAllWithIgnoringAsync()
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            return await context.applications
                .Include(a => a.address).ThenInclude(a => a.district)
                .Include(a => a.address.coordinates)
                .Select(a => new Application(a))
                .Where(a => !a.ignored)
                .ToListAsync();
        }

    }

    public async Task<Application> GetByIdAsync(int id)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            var application = await context.applications
                .Include(a => a.address).ThenInclude(a => a.district)
                .Include(a => a.address.coordinates)
                .Select(a => new Application(a))
                .FirstOrDefaultAsync(a => a.id == id);

            if (application is not null)
            {
                return application;
            }
            else
            {
                throw new Exception($"Application with id {id} not found");
            }
        }
    }

    public async Task UpdateAsync(Application entity)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            var application = await context.applications
                .Include(a => a.address).ThenInclude(a => a.district)
                .Include(a => a.address.coordinates)
                .FirstOrDefaultAsync(a => a.id == entity.id);

            if (application == null)
            {
                throw new Exception($"Application with id {entity.id} not found");
            }
            else
            {
                context.Entry(application).CurrentValues.SetValues(entity);
                if (entity.address != null)
                {
                    application.address = entity.address;
                }
                await context.SaveChangesAsync();
            }
        }
    }

    public Task UpdateRangeAsync(IEnumerable<Application> entities)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            context.applications.UpdateRange(entities);
            return context.SaveChangesAsync();
        }
    }
}
