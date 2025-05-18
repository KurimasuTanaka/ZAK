using ZAK.Db;
using ZAK.Db.Models;

using Microsoft.EntityFrameworkCore;

namespace ZAK.DA;

public class CoordinatesDataAccess(ZakDbContext ZakDbContext) : ICoordinatesDataAccess
{
    private readonly ZakDbContext _ZakDbContext = ZakDbContext;

    public async Task<List<AddressCoordinates>> GetCoordinatesAsync()
    {
        return await _ZakDbContext.coordinates.Select(c => new AddressCoordinates(c)).ToListAsync();
    }

    public async Task<AddressCoordinates> GetCoordinatesByIdAsync(int id)
    {
        AddressCoordinatesModel addressCoordinatesModel = await _ZakDbContext.coordinates.FindAsync(id);

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
        _ZakDbContext.coordinates.Add(coordinates);
        await _ZakDbContext.SaveChangesAsync();
        return coordinates;
    }

    public async Task UpdateCoordinatesAsync(AddressCoordinates coordinates)
    {
        if (_ZakDbContext.coordinates.Find(coordinates.addressId) == null)
        {
            try
            {
                _ZakDbContext.coordinates.Add(coordinates);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        else
        {
            AddressCoordinatesModel coordinatesModel = await _ZakDbContext.coordinates.FindAsync(coordinates.addressId);
            coordinatesModel.lat = coordinates.lat;
            coordinatesModel.lon = coordinates.lon;
        }

        await _ZakDbContext.SaveChangesAsync();
    }
}
