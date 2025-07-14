using System;
using System.Numerics;
using ZAK.DA;
using Itinero;
using Itinero.IO.Osm;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
namespace ZAK.MapRoutesManager;

public class MapRoutesManager : IMapRoutesManager
{
    private RouterDb _routerDb = new RouterDb();
    private Router _router;

    private readonly ILogger<MapRoutesManager> _logger;
    private readonly IBrigadeRepository _brigadeRepository;
    private readonly IMemoryCache _cache;
    public MapRoutesManager(ILogger<MapRoutesManager> logger, IBrigadeRepository brigadeRepository, IConfiguration configuration, IMemoryCache cache)
    {
        _logger = logger;
        _brigadeRepository = brigadeRepository;
        _cache = cache;

        _logger.LogInformation("Loading OSM data...");

        string? osmFilePath = configuration["OsmFilePath"];
        if (string.IsNullOrEmpty(osmFilePath))
        {
            _logger.LogError("OSM file path is not configured.");
            throw new ArgumentException("OSM file path is not configured.");
        }

        try
        {
            using (var stream = new FileInfo(osmFilePath).OpenRead())
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
        if (_routerDb == null || _router == null)
        {
            _logger.LogError("RouterDb or Router is not initialized. Probably OSM data was not loaded correctly.");
            return new List<List<Vector2>>();
        }

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
            addresses.Last().AddRange(brigade.scheduledApplications.Select(a => a.application).
                Where(a => a.address is not null).Select(a => new Address(a.address!)).
                ToList());
        }

        return addresses;
    }

    private List<Vector2> GetPath(List<Address> addressList)
    {
        List<Vector2> path = new List<Vector2>();

        var vehicle = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();

        for (int i = 0; i < addressList.Count - 1; i++)
        {
            float fromLat = (float)addressList[i].coordinates!.lat;
            float fromLon = (float)addressList[i].coordinates!.lon;
            float toLat = (float)addressList[i + 1].coordinates!.lat;
            float toLon = (float)addressList[i + 1].coordinates!.lon;

            if (_cache.TryGetValue($"Route_{fromLat}_{fromLon}_{toLat}_{toLon}", out List<Vector2>? cachedPath) && cachedPath is not null)
            {
                _logger.LogInformation("Using cached path for addresses: {From} to {To}", addressList[i].coordinates, addressList[i + 1].coordinates);
                path.AddRange(cachedPath);
                continue;
            }

            var start = _router.TryResolve(vehicle, fromLat, fromLon, 150);
            var end = _router.TryResolve(vehicle, toLat, toLon, 150);

            if (start.IsError || end.IsError)
            {
                throw new Exception("Error while resolving address");
            }

            var route = _router.TryCalculate(vehicle, start.Value, end.Value);
            if (route.IsError)
            {
                path.Add(new Vector2(fromLat, fromLon));
                path.Add(new Vector2(toLat, toLon));
            }
            else
            {
                List<Vector2> routePath = route.Value.Shape.Select(s => new Vector2(s.Latitude, s.Longitude)).ToList();

                _cache.Set($"Route_{fromLat}_{fromLon}_{toLat}_{toLon}", routePath, TimeSpan.FromHours(1));

                path.AddRange(routePath);
            }
        }

        return path;
    }

    public bool CheckResolving(float lat, float lon, float radius = 150)
    {
        if (_routerDb == null || _router == null)
        {
            _logger.LogError("RouterDb or Router is not initialized. Probably OSM data was not loaded correctly.");
            return false;
        }

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
        if (_routerDb == null || _router == null)
        {
            _logger.LogError("RouterDb or Router is not initialized. Probably OSM data was not loaded correctly.");
            return false;
        }

        var vehicle = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();

        if (address.coordinates == null)
        {
            _logger.LogError("Address coordinates are null. Cannot resolve address.");
            return false;
        }

        var result = _router.TryResolve(vehicle, (float)address.coordinates!.lat, (float)address.coordinates!.lon, radius);
        if (result.IsError)
        {
            return false;
        }
        return true;
    }

    public bool CheckConnection(Address address, float radius = 50)
    {
        if(_routerDb == null || _router == null)
        {
            _logger.LogError("RouterDb or Router is not initialized. Probably OSM data was not loaded correctly.");
            return false;
        }

        var vehicle = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();

        bool result = _router.CheckConnectivity(vehicle, new RouterPoint((float)address.coordinates!.lat, (float)address.coordinates!.lon, 0, 0), radius);
        if (result)
        {
            return false;
        }
        return true;
    }
}
