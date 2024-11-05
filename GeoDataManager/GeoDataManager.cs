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

        for (int i = 0; i < addresses.Count; i++)
        {
            addresses[i].coordinates = new AddressesCoordinates();
            await GetCoordinatesForAddress(addresses[i]);
        }

        await _addressesDataAccess.UpgdateAddresses(addresses);
    }

    public async Task GetCoordinatesForAddress(Address address)
    {
        string addressString =
            "Київ+" +
            address.streetName.Replace(' ', '+') + "+" +
            address.building;

        string url = "https://nominatim.openstreetmap.org/search?format=json&q=" + addressString;

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

        HttpClient client = new HttpClient();



        HttpResponseMessage response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            string responseString = await response.Content.ReadAsStringAsync();

            JsonNode jsonNode = JsonNode.Parse(responseString);

            address.coordinates.lat = Double.Parse(jsonNode[0]["lat"].ToString());
            address.coordinates.lon = Double.Parse(jsonNode[0]["lon"].ToString());
        }

        Task.Delay(1100).Wait();
    }
}
