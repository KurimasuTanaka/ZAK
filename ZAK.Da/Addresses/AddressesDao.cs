using System;
using BlazorApp.DA;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZAK.Da.BaseDAO;
using ZAK.Db;
using ZAK.Db.Models;

namespace ZAK.Da.Addresses;

public class AddressesDao : IDaoBase<Address, AddressModel>
{
    private BlazorAppDbContext  _dbContext;
    private ILogger<AddressesDao> _logger;

    public AddressesDao(BlazorAppDbContext dbContext, ILogger<AddressesDao> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation($"Deleting address with id: {id}");

        AddressModel? addressModel = await _dbContext.addresses.FindAsync(id);

        if(addressModel is null)
        {
            _logger.LogWarning($"Address with id: {id} not found");
        }
        else 
        {
            _logger.LogInformation($"Address with id: {id} found. Deleting...");
            _dbContext.addresses.Remove(addressModel);
            await _dbContext.SaveChangesAsync();
        }

    }

    public async Task<List<Address>> GetAll()
    {
        _logger.LogInformation("Getting all addresses");
        return await _dbContext.addresses.Select(a => new Address(a)).ToListAsync();
    }

    public async Task<Address> GetById(int id)
    {
        AddressModel? addressModel = await _dbContext.addresses.FindAsync(id);

        if(addressModel is null)
        {
            _logger.LogWarning($"Address with id: {id} not found");
            return new Address();
        }
        else 
        {
            _logger.LogInformation($"Address with id: {id} found. Deleting...");
            return new Address(addressModel);
        }
    }

    public async Task Insert(Address entity)
    {
        _logger.LogInformation("Inserting new address");
        await _dbContext.addresses.AddAsync(entity);
    }

    public async Task Update(Address entity, int id)
    {
        AddressModel? addressModel = await _dbContext.addresses.FindAsync(entity.Id);

        if(addressModel is null)
        {
            _logger.LogWarning($"Address with id: {entity.Id} not found. Adding new address...");
            await _dbContext.addresses.AddAsync(entity);
        }
        else 
        {
            _logger.LogInformation($"Address with id: {entity.Id} found. Updating...");
            addressModel.addressAlias = entity.addressAlias;
            addressModel.addressPriority = entity.addressPriority;
            addressModel.coordinates = entity.coordinates;
            addressModel.building = entity.building;
            addressModel.streetName = entity.streetName;
            addressModel.districtName = entity.districtName;
            await _dbContext.SaveChangesAsync();
        }
    
    }
}
