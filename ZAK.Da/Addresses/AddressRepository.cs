using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ZAK.Db;

namespace ZAK.DA;

public class AddressRepository : IAddressRepository
{
    private readonly IDbContextFactory<ZakDbContext> _dbContextFactory;

    public AddressRepository(IDbContextFactory<ZakDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task CreateAsync(Address entity)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            context.addresses.Add(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            var entity = context.addresses.Find(id);
            if (entity != null)
            {
                context.addresses.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }

    public async Task<IEnumerable<Address>> GetAllAsync()
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            return await context.addresses.Include(a => a.addressAlias)
                                          .Include(a => a.addressPriority)
                                          .Include(a => a.coordinates)
                                          .Select(a => new Address(a))
                                          .ToListAsync();
        }
    }

    public async Task<Address> GetByIdAsync(int id)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            Address? address = await context.addresses
                                          .Include(a => a.addressAlias)
                                          .Include(a => a.addressPriority)
                                          .Include(a => a.coordinates)
                                          .Select(a => new Address(a))
                                          .FirstOrDefaultAsync(a => a.Id == id);
            if (address == null)
            {
                throw new Exception($"Address with id {id} not found");
            }
            return address;
        }
    }

    public async Task CreateRangeAsync(IEnumerable<Address> addresses)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            List<District> districts = await context.districts.Select(d => new District(d)).ToListAsync();

            foreach (var address in addresses)
            {
                District? districtFromDb = districts.Find(dist =>
                                        {
                                            if (address.district is not null && dist.name == address.district.name)
                                            {
                                                return true;
                                            }
                                            return false;
                                        });

                if (districtFromDb is not null)
                {
                    address.district = districtFromDb;
                    context.Attach(address.district);
                }
            }
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(Address entity)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            var existingEntity = await context.addresses.FindAsync(entity.Id);
            if (existingEntity != null)
            {
                context.Entry(existingEntity).CurrentValues.SetValues(entity);
                await context.SaveChangesAsync();
            }
            else 
            {
                throw new Exception($"Address with id {entity.Id} not found");
            }
        }
    }
}
