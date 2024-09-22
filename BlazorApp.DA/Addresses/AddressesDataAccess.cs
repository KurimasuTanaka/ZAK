using System;
using BlazorApp.DB;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.DA.Addresses;

public class AddressesDataAccess(BlazorAppDbContext dbContext) : IAddressesDataAccess
{
    private  readonly BlazorAppDbContext _dbContext = dbContext;

    public Task AddAddress(Address address)
    {
        _dbContext.addresses.Add(address);
        return _dbContext.SaveChangesAsync();
    }

    public async Task<List<Address>> GetAddresses()
    {
        return await _dbContext.addresses.Select(a => new Address(a)).ToListAsync(); 
    }

    public async Task<double> GetPriorityByAddress(string streetName, string building)
    {
        AddressModel? address = await _dbContext.addresses.
            Where(a => a.building == building).
            Where( a => a.streetName == streetName).FirstOrDefaultAsync();

        if(address is null)
        {
            return 0.0;
        } else return address.priority;
    }

    public async Task UpdatePriority(int id, double priority)
    {
        AddressModel? address = await _dbContext.addresses.FindAsync(id);

        if(address is not null)
        {
            address.priority = priority;
            await _dbContext.SaveChangesAsync();
        } 
    }
}
