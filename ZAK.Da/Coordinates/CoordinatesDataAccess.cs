using ZAK.Db;
using ZAK.Db.Models;

using Microsoft.EntityFrameworkCore;

namespace ZAK.DA;

public class CoordinatesDataAccess(BlazorAppDbContext blazorAppDbContext) : ICoordinatesDataAccess
{
    private readonly BlazorAppDbContext _blazorAppDbContext = blazorAppDbContext;

    public async Task<List<AddressCoordinates>> GetCoordinatesAsync()
    {
        return await _blazorAppDbContext.coordinates.Select(c => new AddressCoordinates(c)).ToListAsync();
    }

    public async Task<AddressCoordinates> GetCoordinatesByIdAsync(int id)
    {
        AddressCoordinatesModel addressCoordinatesModel = await _blazorAppDbContext.coordinates.FindAsync(id);

        if (addressCoordinatesModel == null)
        {
            return null;
        }
        else
        {
            return new AddressCoordinates(addressCoordinatesModel);
        }
    }

    public async Task<AddressCoordinates> AddCoordinatesAsync(AddressCoordinates coordinates)
    {
        _blazorAppDbContext.coordinates.Add(coordinates);
        await _blazorAppDbContext.SaveChangesAsync();
        return coordinates;
    }

    public async Task UpdateCoordinatesAsync(AddressCoordinates coordinates)
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
            AddressCoordinatesModel coordinatesModel = await _blazorAppDbContext.coordinates.FindAsync(coordinates.addressId);
            coordinatesModel.lat = coordinates.lat;
            coordinatesModel.lon = coordinates.lon;
        }

        await _blazorAppDbContext.SaveChangesAsync();
    }
}
