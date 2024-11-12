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

            JsonNode? jsonNode = JsonNode.Parse(responseString!);
            if(jsonNode is null) return;
            
            if(jsonNode.AsArray().Count != 1) return;

            string lat = jsonNode[0]!.AsObject().TryGetPropertyValue("lat", out var latValue) ? latValue!.ToString() : "0.0";
            string lon = jsonNode[0]!.AsObject().TryGetPropertyValue("lon", out var lonValue) ? lonValue!.ToString() : "0.0";

            address.coordinates.lat = Double.Parse(lat);
            address.coordinates.lon = Double.Parse(lon);
        }

        Task.Delay(1100).Wait();
    }
}
