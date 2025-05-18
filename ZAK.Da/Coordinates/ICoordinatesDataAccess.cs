using System;

namespace ZAK.DA;

public interface ICoordinatesDataAccess
{
    public Task<List<AddressCoordinates>> GetCoordinatesAsync();
    public Task<AddressCoordinates> GetCoordinatesByIdAsync(int id);
    public Task<AddressCoordinates> AddCoordinatesAsync(AddressCoordinates coordinates);
    public Task UpdateCoordinatesAsync(AddressCoordinates coordinates);

}
