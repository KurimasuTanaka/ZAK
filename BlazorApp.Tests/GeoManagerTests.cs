using BlazorApp.DA;
using BlazorApp.DB;
using BlazorApp.GeoDataManager;

namespace BlazorApp.Tests;

public class GeoDataManagerTests
{

    [Fact]
    public async void Proper_Address_Loaded_Coordinates_Received()
    {
        // Arrange
        ICoordinatesProvider _coordinatesProvider = new NominatimCoordinatesProvider();

        
        var address = new Address
        {
            streetName = "Київ проспект Володимира Івасюка",
            building = "54"
        };

        address.coordinates = new Coordinates();

        // Act

        await _coordinatesProvider.GetCoordinatesForAddress(address);

        // Assert
        Assert.Equal(50.52418025, address.coordinates.lat);
        Assert.Equal(30.51606932744258, address.coordinates.lon);
    }
}