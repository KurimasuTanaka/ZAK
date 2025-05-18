using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ZAK.DA;
using Microsoft.EntityFrameworkCore;
using ZAK.DAO;
using ZAK.Db.Models;

namespace BlazorApp.GeoDataManager;


public class GeoDataManager : IGeoDataManager
{
    IAddressRepository _addressRepository;
    ICoordinatesProvider? _coordinatesProvider = new NominatimCoordinatesProvider();


    public GeoDataManager(IAddressRepository addressRepository)
    {
        _addressRepository = addressRepository;
    }

    public async Task PopulateApplicationsWithGeoData()
    {
        List<Address> addresses = (await _addressRepository.GetAllAsync()).Where(a => a.coordinates is null || a.coordinates.lat == 0 || a.coordinates.lon == 0).ToList();

        foreach (Address address in addresses)
        {
            if (address.coordinates is null) address.coordinates = new AddressCoordinates();
            await _coordinatesProvider!.GetCoordinatesForAddress(address);
        }

        await _addressRepository.UpdateRangeAsync(addresses);
    }

}
