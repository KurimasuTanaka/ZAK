using System;
using ZAK.Db;
using ZAK.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace ZAK.DA;

public class AddressAliasDataAccess(BlazorAppDbContext blazorAppDbContext) : IAddressAliasDataAccess
{
    BlazorAppDbContext _context = blazorAppDbContext; 
    public Task AddAddressAlias(AddressAlias addressAlias)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAddressAlias(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<AddressAlias>> GetAddressAliases()
    {
        return await _context.addressAliases.Select(a => new AddressAlias(a)).ToListAsync();
    }

    public async Task UpdateAddressSteetAlias(int id, string streetAlias)
    {
        AddressAliasModel? addressAlias = await _context.addressAliases.FindAsync(id);
        if(addressAlias is not null)
        {
            addressAlias.streetAlias = streetAlias;
            await _context.SaveChangesAsync();
        } else 
        {
            addressAlias = new AddressAliasModel() { streetAlias = streetAlias, addressId = id };   
            await _context.addressAliases.AddAsync(addressAlias);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task UpdateAddressBuildingAlias(int id, string buildingAlias)
    {
        AddressAliasModel? addressAlias = await _context.addressAliases.FindAsync(id);
        if(addressAlias is not null)
        {
            addressAlias.buildingAlias = buildingAlias;
            await _context.SaveChangesAsync();
        } else 
        {
            addressAlias = new AddressAliasModel() { buildingAlias = buildingAlias, addressId = id };   
            await _context.addressAliases.AddAsync(addressAlias);
            await _context.SaveChangesAsync();
        }
    }
}
