using System;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using ZAK.DA;

namespace BlazorApp.GeoDataManager;

public class NominatimCoordinatesProvider : ICoordinatesProvider
{
    public async Task GetCoordinatesForAddress(Address address)
    {
        string addressString = "Київ+";

        if (address.addressAlias is not null)
        {
            if (address.addressAlias.streetAlias != "" && address.addressAlias.streetAlias != null)
            {
                addressString += address.addressAlias.streetAlias.Replace(' ', '+') + "+";
            }
            else addressString += address.streetName.Replace(' ', '+') + "+";

            if (address.addressAlias.buildingAlias != "" && address.addressAlias.buildingAlias != null)
            {
                addressString += address.addressAlias.buildingAlias.Replace(' ', '+') + "+";
            }
            else addressString += address.building.Replace(' ', '+') + "+";

        }
        else addressString +=
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
            if (jsonNode is null) return;

            if (jsonNode.AsArray().Count > 1)
            {
                if (jsonNode[0]!.AsObject().TryGetPropertyValue("addresstype", out var addresstype)) ;
                if (addresstype is not null)
                {
                    string addresstypeString = addresstype.ToString();
                    if (!addresstypeString.Equals("building")) return;
                }
            }
            else if (jsonNode.AsArray().Count == 0) return;

            string lat = jsonNode[0]!.AsObject().TryGetPropertyValue("lat", out var latValue) ? latValue!.ToString() : "0.0";
            string lon = jsonNode[0]!.AsObject().TryGetPropertyValue("lon", out var lonValue) ? lonValue!.ToString() : "0.0";

            address.coordinates.lat = Double.Parse(lat);
            address.coordinates.lon = Double.Parse(lon);
        }

        Task.Delay(1100).Wait();
    }
}
