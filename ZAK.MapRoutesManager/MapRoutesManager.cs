using System;
using System.Numerics;
using ZAK.DA;
using Itinero;
using Itinero.IO.Osm;
using Microsoft.Extensions.Logging;
namespace ZAK.MapRoutesManager;

public class MapRoutesManager : IMapRoutesManager
{
    private RouterDb _routerDb = new RouterDb();
    private Router _router;

    private ILogger<MapRoutesManager> _logger;
    private readonly IBrigadeRepository _brigadeRepository;

    public MapRoutesManager(ILogger<MapRoutesManager> logger, IBrigadeRepository brigadeRepository)
    {
        _logger = logger;
        _brigadeRepository = brigadeRepository;

        _logger.LogInformation("Loading OSM data...");

        try
        {
            using (var stream = new FileInfo(@"./../../kyiv.osm.pbf").OpenRead())
            {
                _routerDb.LoadOsmData(stream, Itinero.Osm.Vehicles.Vehicle.Car); // create the network for cars only.
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading OSM data");
            throw;
        }
        _router = new Itinero.Router(_routerDb);

    }

    //For testing purposes
    public MapRoutesManager(string osmPdbFilePath, ILogger<MapRoutesManager> logger, IBrigadeRepository brigadeRepository)
    {
        _logger = logger;
        _brigadeRepository = brigadeRepository;

        _logger.LogInformation("Loading OSM data...");

        try
        {
            using (var stream = new FileInfo(osmPdbFilePath).OpenRead())
            {
                _routerDb.LoadOsmData(stream, Itinero.Osm.Vehicles.Vehicle.Car); // create the network for cars only.
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading OSM data");
            throw;
        }
        _router = new Itinero.Router(_routerDb);

    }


    public async Task<List<List<Vector2>>> GetRoutesAsync()
    {

        _logger.LogInformation("Populationg brigades with applications");
        //Populate brigades with applications
        List<Brigade> brigades = (await _brigadeRepository.GetAllWithScheduledApplicationInfoAsync()).ToList();

        _logger.LogInformation("Calculating routes...");

        //Calculate routes 
        List<List<Vector2>> routes = new List<List<Vector2>>();

        foreach (List<Address> addressList in GetAddresses(brigades))
        {
            //Check if there is only one address in the list or if the list is empty
            if (addressList.Count == 1 || addressList.Count == 0) continue;
        
            routes.Add(GetPath(addressList));
        }

        return routes;
    }

    private List<List<Address>> GetAddresses(List<Brigade> brigades)
    {
        List<List<Address>> addresses = new List<List<Address>>();

        foreach (Brigade brigade in brigades)
        {
            addresses.Add(new List<Address>());
            addresses.Last().AddRange(brigade.scheduledApplications.Select(a => a.application).Select(a => new Address(a.address)).ToList());
        }

        return addresses;
    }

    private List<Vector2> GetPath(List<Address> addressList)
    {
        List<Vector2> path = new List<Vector2>();

        var vehicle = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();

        for (int i = 0; i < addressList.Count - 1; i++)
        {
            var start = _router.TryResolve(vehicle, (float)addressList[i].Latitude, (float)addressList[i].Longtitude, 150);
            var end = _router.TryResolve(vehicle, (float)addressList[i + 1].Latitude, (float)addressList[i + 1].Longtitude, 150);
            if (start.IsError || end.IsError)
            {
                throw new Exception("Error while resolving address");
            }

            var route = _router.TryCalculate(vehicle, start.Value, end.Value);
            if (route.IsError)
            {
                path.Add(new Vector2((float)addressList[i].Latitude, (float)addressList[i].Longtitude));
                path.Add(new Vector2((float)addressList[i + 1].Latitude, (float)addressList[i + 1].Longtitude));
            }
            else path.AddRange(route.Value.Shape.Select(s => new Vector2(s.Latitude, s.Longitude)));
        }

        return path;
    }

    public bool CheckResolving(float lat, float lon, float radius = 150)
    {
        var vehicle = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();

        var result = _router.TryResolve(vehicle, lat, lon, radius);
        if (result.IsError)
        {
            return false;
        }
        return true;
    }

    public bool CheckResolving(Address address, float radius = 150)
    {
        var vehicle = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();

        var result = _router.TryResolve(vehicle, (float)address.Latitude, (float)address.Longtitude, radius);
        if (result.IsError)
        {
            return false;
        }
        return true;
    }

    public bool CheckConnection(Address address, float radius = 50)
    {
        var vehicle = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();

        bool result = _router.CheckConnectivity(vehicle, new RouterPoint((float)address.Latitude, (float)address.Longtitude, 0, 0), radius);
        if (result)
        {
            return false;
        }
        return true;
    }
}
