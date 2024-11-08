using System;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using BlazorApp.DA;

namespace BlazorApp.GeoDataManager;

public class NominatimCoordinatesProvider : ICoordinatesProvider
{
    public async Task GetCoordinatesForAddress(Address address)
    {
        string addressString = String.Empty;

        if (address.addressAlias is not null)
        {
            addressString =
                "Київ+" +
                address.addressAlias.streetAlias.Replace(' ', '+') + "+" +
                address.addressAlias.buildingAlias;
        } addressString =
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
