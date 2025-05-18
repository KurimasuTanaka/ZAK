using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using ZAK.Db;
using ZAK.Db.Models;

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

            await context.addresses.AddRangeAsync(addresses);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(Address entity)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            var existingEntity = await context.addresses.Include(a => a.coordinates).Include(a => a.addressAlias).Include(a => a.addressPriority).FirstOrDefaultAsync(a => a.Id == entity.Id);
            if (existingEntity != null)
            {
                context.Entry(existingEntity).CurrentValues.SetValues(entity);
                if (entity.coordinates != null) existingEntity.coordinates = entity.coordinates;
                if (entity.addressAlias != null) existingEntity.addressAlias = entity.addressAlias;
                if (entity.addressPriority != null) existingEntity.addressPriority = entity.addressPriority;

                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Address with id {entity.Id} not found");
            }
        }
    }

    public Task UpdateRangeAsync(IEnumerable<Address> addresses)
    {
        using (ZakDbContext context = _dbContextFactory.CreateDbContext())
        {
            List<AddressModel> existingAddresses = context.addresses.Include(a => a.coordinates).Include(a => a.addressAlias).Include(a => a.addressPriority).ToList();
            foreach (var address in addresses)
            {
                AddressModel? existingEntity = existingAddresses.FirstOrDefault(a => a.Id == address.Id);
                if (existingEntity == null)
                {
                    throw new Exception($"Address with id {address.Id} not found");
                }

                context.Entry(existingEntity).CurrentValues.SetValues(address);

                if (address.coordinates != null) existingEntity.coordinates = address.coordinates;
                if (address.addressAlias != null) existingEntity.addressAlias = address.addressAlias;
                if (address.addressPriority != null) existingEntity.addressPriority = address.addressPriority;

            }
            return context.SaveChangesAsync();
        }
    }
}
