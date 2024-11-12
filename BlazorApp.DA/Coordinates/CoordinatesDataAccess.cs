using System;
using BlazorApp.DB;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.DA;

public class CoordinatesDataAccess(BlazorAppDbContext blazorAppDbContext) : ICoordinatesDataAccess
{
    private readonly BlazorAppDbContext _blazorAppDbContext = blazorAppDbContext;

    public async Task<List<Coordinates>> GetCoordinatesAsync()
    {
        return await _blazorAppDbContext.coordinates.Select(c => new Coordinates(c)).ToListAsync();
    }

    public async Task<Coordinates> GetCoordinatesByIdAsync(int id)
    {
        AddressCoordinatesModel addressCoordinatesModel = await _blazorAppDbContext.coordinates.FindAsync(id);

        if (addressCoordinatesModel == null)
        {
            return null;
        }
        else
        {
            return new Coordinates(addressCoordinatesModel);
        }
    }

    public async Task<Coordinates> AddCoordinatesAsync(Coordinates coordinates)
    {
        _blazorAppDbContext.coordinates.Add(coordinates);
        await _blazorAppDbContext.SaveChangesAsync();
        return coordinates;
    }

    public async Task UpdateCoordinatesAsync(Coordinates coordinates)
    {
        if (_blazorAppDbContext.coordinates.Find(coordinates.addressId) == null)
        {
            try
            {
                _blazorAppDbContext.coordinates.Add(coordinates);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        else
        {
            _blazorAppDbContext.Entry(coordinates).State = EntityState.Modified;
        }

        await _blazorAppDbContext.SaveChangesAsync();
    }
}
