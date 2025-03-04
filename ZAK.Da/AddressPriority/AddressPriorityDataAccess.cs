using System;
using ZAK.Db;
using ZAK.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.DA;

public class AddressPriorityDataAccess(BlazorAppDbContext blazorAppDbContext) : IAddressPriorityDataAccess
{
    private readonly BlazorAppDbContext _dbContext = blazorAppDbContext;

    public async Task<List<AddressPriority>> GetAddressPriorities()
    {
        return await _dbContext.addressPriorities.Include(a => a.address).Select(a => new AddressPriority(a)).ToListAsync();
    }

    public async Task<double> GetAddressPriority(int id)
    {
        AddressPriorityModel? addressPriority = await _dbContext.addressPriorities.FindAsync(id);

        if(addressPriority is null) return 0.0;
        else return addressPriority.priority;
    }

    public async Task UpdatePriority(int id, double priority)
    {
        AddressPriorityModel? addressPriority = await _dbContext.addressPriorities.FindAsync(id);

        if(addressPriority is not null)
        {
            addressPriority.priority = priority;
            await _dbContext.SaveChangesAsync();
        } 
    }
}
