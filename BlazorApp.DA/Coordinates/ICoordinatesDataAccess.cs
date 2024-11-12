using System;

namespace BlazorApp.DA;

public interface ICoordinatesDataAccess
{
    public Task<List<Coordinates>> GetCoordinatesAsync();
    public Task<Coordinates> GetCoordinatesByIdAsync(int id);
    public Task<Coordinates> AddCoordinatesAsync(Coordinates coordinates);
    public Task UpdateCoordinatesAsync(Coordinates coordinates);

}
