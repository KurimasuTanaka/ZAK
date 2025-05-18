using System;
using ZAK.Db;
using ZAK.Db.Models;
using Microsoft.EntityFrameworkCore;
using BlazorApp.Enums;

namespace ZAK.DA;

public class AddressesDataAccess(ZakDbContext dbContext) : IAddressesDataAccess
{
    private  readonly ZakDbContext _dbContext = dbContext;

    public async Task AddAddress(Address address)
    {
        _dbContext.addresses.Add(address);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Address>> GetAddresses()
    {
        return await _dbContext.addresses.Include(a => a.addressPriority).Include(a => a.addressAlias).Include(a => a.coordinates).Select(a => new Address(a)).ToListAsync(); 
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
        return await _dbContext.addresses.Include(a => a.addressAlias).
            Where(a => a.coordinates == null || a.coordinates.lat == 0||a.coordinates.lon == 0 ).
            Select(a => new Address(a)).ToListAsync(); 

    }

    public async Task UpgdateAddresses(List<Address> addresses)
    {
        List<AddressModel> addressModels = await _dbContext.addresses.ToListAsync();

        foreach (var addressModel in addressModels)
        {
            Address? address = addresses.Where(a => a.Id == addressModel.Id).FirstOrDefault();
        
            if(address is not null)
            {
                if(address.addressAlias is not null) addressModel.addressAlias = address.addressAlias;
                if(address.addressPriority is not null) addressModel.addressPriority = address.addressPriority;
                if(address.coordinates is not null) addressModel.coordinates = address.coordinates;
                addressModel.building = addressModel.building;
                addressModel.streetName = addressModel.streetName;
                //addressModel.districtName = addressModel.districtName;
            }        
        }

        await _dbContext.SaveChangesAsync();
        return;
    }

    public async Task SetBlackoutGroup(int id, int group)
    {
        AddressModel? address = await _dbContext.addresses.FindAsync(id);
        if(address is not null)
        {
            address.blackoutGroup = group;
            await _dbContext.SaveChangesAsync();
        }
        else return;
    }

    public async Task UpdateEquipmentAccess(int id, EquipmentAccess access)
    {
        AddressModel? address = await _dbContext.addresses.FindAsync(id);
        if(address is not null)
        {
            address.equipmentAccess = access;
            await _dbContext.SaveChangesAsync();
        }
        else return;
    }
}
