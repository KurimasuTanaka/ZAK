using System;
using BlazorApp.DB;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.DA;

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
        return await _dbContext.addresses.Include(a => a.addressPriority).Include(a => a.addressAlias).Select(a => new Address(a)).ToListAsync(); 
    }

    public async Task<double> GetPriorityByAddress(string streetName, string building)
    {
        AddressModel? address = await _dbContext.addresses.
            Where(a => a.building == building).
            Where( a => a.streetName == streetName).FirstOrDefaultAsync();

        if(address is null)
        {
            return 0.0;
        } else return address.addressPriority is null ? 0.0 : address.addressPriority.priority;
    }

    public async Task UpdatePriority(int id, double priority)
    {

        throw new Exception("Another method should be used!!!");

        AddressModel? address = await _dbContext.addresses.FindAsync(id);

        AddressPriorityModel? addressPriority = await _dbContext.addressPriorities.FindAsync(id);

        if(address is not null)
        {
            if(address.addressPriority is null)
            {
                address.addressPriority = new AddressPriorityModel();
                address.addressPriority.priority = priority;
                address.addressPriority.addressId = id;
            }
            else address.addressPriority.priority = priority;
            await _dbContext.SaveChangesAsync();
        } 
    }
    public async Task<List<Address>> GetAddressesWithoutLocation()
    {
        return await _dbContext.addresses.Include(a => a.addressAlias).Where(a => a.coordinates == null).Select(a => new Address(a)).ToListAsync(); 

    }

    public Task UpgdateAddresses(List<Address> addresses)
    {
        _dbContext.addresses.UpdateRange(addresses);

        return _dbContext.SaveChangesAsync();
    }
}
