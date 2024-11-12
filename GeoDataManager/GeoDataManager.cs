using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using BlazorApp.DA;
using BlazorApp.DB;

namespace BlazorApp.GeoDataManager;
public class GeoDataManager : IGeoDataManager
{
    IAddressesDataAccess? _addressesDataAccess = null;
    ICoordinatesProvider? _coordinatesProvider = new NominatimCoordinatesProvider();

    public GeoDataManager()
    {
    }

    public GeoDataManager(IAddressesDataAccess addressesDataAccess)
    {
        _addressesDataAccess = addressesDataAccess;
    }

    public async Task PopulateApplicationsWithGeoData()
    {
        List<Address> addresses = await _addressesDataAccess.GetAddressesWithoutLocation();

        for (int i = 0; i < 20; i++)
        {
            addresses[i].coordinates = new Coordinates();
            await _coordinatesProvider!.GetCoordinatesForAddress(addresses[i]);
        }

        await _addressesDataAccess.UpgdateAddresses(addresses);
    }

}
