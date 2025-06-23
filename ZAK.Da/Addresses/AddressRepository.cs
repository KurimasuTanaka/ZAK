using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.DA;

public class AddressRepository : IAddressRepository
{
    private readonly IDbContextFactory<ZakDbContext> _dbContextFactory;
    private readonly ILogger<AddressRepository> _logger;

    public AddressRepository(IDbContextFactory<ZakDbContext> dbContextFactory, ILogger<AddressRepository> logger)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    public async Task CreateAsync(Address entity)
    {
        _logger.LogInformation("Creating address: {@Address}", entity);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                context.addresses.Add(entity);
                await context.SaveChangesAsync();
            }
            _logger.LogInformation("Address created successfully: {@Address}", entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating address: {@Address}", entity);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting address with id: {Id}", id);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var entity = context.addresses.Find(id);
                if (entity != null)
                {
                    context.addresses.Remove(entity);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Address deleted successfully: {Id}", id);
                }
                else
                {
                    _logger.LogWarning("Address with id {Id} not found for deletion", id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address with id: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Address>> GetAllAsync()
    {
        _logger.LogInformation("Getting all addresses");

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var result = await context.addresses.Include(a => a.addressAlias)
                                                    .Include(a => a.addressPriority)
                                                    .Include(a => a.coordinates)
                                                    .Include(a => a.district)
                                                    .Select(a => new Address(a))
                                                    .ToListAsync();
                _logger.LogInformation("Retrieved {Count} addresses", result.Count);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all addresses");
            throw;
        }
    }

    public async Task<Address> GetByIdAsync(int id)
    {
        _logger.LogInformation("Getting address by id: {Id}", id);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var address = await context.addresses
                                                .Include(a => a.addressAlias)
                                                .Include(a => a.addressPriority)
                                                .Include(a => a.coordinates)
                                                .FirstOrDefaultAsync(a => a.Id == id);
                                                
                if (address == null)
                {
                    _logger.LogWarning("Address with id {Id} not found", id);
                    throw new Exception($"Address with id {id} not found");
                }
                _logger.LogInformation("Address retrieved: {@Address}", address);
                return new Address(address);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting address by id: {Id}", id);
            throw;
        }
    }

    public async Task CreateRangeAsync(IEnumerable<Address> addresses)
    {
        _logger.LogInformation("Creating range of addresses. Count: {Count}", addresses.Count());

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                // Get all districts from DB as entities (not as new District(d)), so EF tracks them
                var districts = await context.districts.ToListAsync();

                foreach (var address in addresses)
                {
                    if (address.district != null)
                    {
                        // Try to find existing district by name
                        var existingDistrict = districts.FirstOrDefault(d => d.name == address.district.name);
                        if (existingDistrict != null)
                        {
                            // Attach the existing tracked entity to the address
                            address.district = existingDistrict;
                        }
                    }
                }

                await context.addresses.AddRangeAsync(addresses);
                await context.SaveChangesAsync();
            }
            _logger.LogInformation("Range of addresses created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating range of addresses");
            throw;
        }
    }

    public async Task UpdateAsync(Address entity)
    {
        _logger.LogInformation("Updating address: {@Address}", entity);

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                var existingEntity = await context.addresses.Include(a => a.coordinates).Include(a => a.addressAlias).Include(a => a.addressPriority).Include(a => a.district).FirstOrDefaultAsync(a => a.Id == entity.Id);
                if (existingEntity != null)
                {
                    context.Entry(existingEntity).CurrentValues.SetValues(entity);
                    if (entity.coordinates != null) existingEntity.coordinates = entity.coordinates;
                    if (entity.addressAlias != null) existingEntity.addressAlias = entity.addressAlias;
                    if (entity.addressPriority != null) existingEntity.addressPriority = entity.addressPriority;
                    if (entity.district != null) existingEntity.district = context.districts.FirstOrDefault(d => d.name == entity.district.name) ?? entity.district;

                    await context.SaveChangesAsync();
                    _logger.LogInformation("Address updated successfully: {@Address}", entity);
                }
                else
                {
                    _logger.LogWarning("Address with id {Id} not found for update", entity.Id);
                    throw new Exception($"Address with id {entity.Id} not found");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address: {@Address}", entity);
            throw;
        }
    }

    public async Task UpdateRangeAsync(IEnumerable<Address> addresses)
    {
        _logger.LogInformation("Updating range of addresses. Count: {Count}", addresses.Count());

        try
        {
            using (ZakDbContext context = _dbContextFactory.CreateDbContext())
            {
                List<AddressModel> existingAddresses = context.addresses.Include(a => a.coordinates).Include(a => a.addressAlias).Include(a => a.addressPriority).ToList();
                foreach (var address in addresses)
                {
                    AddressModel? existingEntity = existingAddresses.FirstOrDefault(a => a.Id == address.Id);
                    if (existingEntity == null)
                    {
                        _logger.LogWarning("Address with id {Id} not found for update", address.Id);
                        throw new Exception($"Address with id {address.Id} not found");
                    }

                    context.Entry(existingEntity).CurrentValues.SetValues(address);

                    if (address.coordinates != null) existingEntity.coordinates = address.coordinates;
                    if (address.addressAlias != null) existingEntity.addressAlias = address.addressAlias;
                    if (address.addressPriority != null) existingEntity.addressPriority = address.addressPriority;
                }
                await context.SaveChangesAsync();
                _logger.LogInformation("Range of addresses updated successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating range of addresses");
            throw;
        }
    }
}
