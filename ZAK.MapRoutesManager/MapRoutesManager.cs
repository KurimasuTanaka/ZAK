using System;
using System.Numerics;
using BlazorApp.DA;
using Itinero;
using Itinero.IO.Osm;
using Itinero.Osm.Vehicles;
using Itinero.Profiles;
using OsmSharp.API;
namespace ZAK.MapRoutesManager;

public class MapRoutesManager : IMapRoutesManager
{
    private RouterDb _routerDb = new RouterDb();
    private Router _router;

    public MapRoutesManager()
    {
        using (var stream = new FileInfo(@"/C#_Projects\ZAK\ZAK3\kyiv.osm.pbf").OpenRead())
        {
            _routerDb.LoadOsmData(stream, Itinero.Osm.Vehicles.Vehicle.Car); // create the network for cars only.
        }
        _router = new Itinero.Router(_routerDb);

    }

    public async Task<List<List<Vector2>>> GetRoutesAsync(IBrigadesDataAccess brigadesDataAccess, IApplicationsDataAccess applicationsDataAccess)
    {
        //Populate brigades with applications
        List<Brigade> brigades = await brigadesDataAccess.GetAllBrigades();
        foreach (Brigade brigade in brigades)
        {
            brigade.PopulateApplicationList(applicationsDataAccess);
        }

        //Remove empty adresses and create lists scheduled addresses
        List<List<Address>> addresses = new List<List<Address>>();
        foreach (Brigade brigade in brigades)
        {
            addresses.Add(new List<Address>());
            for (int i = 0; i < brigade.applications.Count; i++)
            {
                if (brigade.applications[i] != null)
                {
                    addresses.Last().Add(new Address(brigade.applications[i].address));
                }
            }
        }

        //Calculate routes 
        var vehicle = Itinero.Osm.Vehicles.Vehicle.Car.Fastest();
        List<List<Vector2>> routes = new List<List<Vector2>>();

        foreach (List<Address> addressList in addresses)
        {
            if (addressList.Count == 1 || addressList.Count == 0)
            {
                continue;
            }

            routes.Add(new List<Vector2>());


            // List<RouterPoint> routePoints = new List<RouterPoint>();
            // foreach(Address address in addressList)
            // {
            //     var routerPoint = _router.TryResolve(vehicle, (float)address.Latitude, (float)address.Longtitude, 150);
            //     if (routerPoint.IsError)
            //     {
            //         throw new Exception("Error while resolving address");
            //     }
            //     routePoints.Add(routerPoint.Value);
            // }
            // var route = _router.TryCalculate(vehicle, routePoints.ToArray());
            // if (route.IsError)
            // {
            //     routes.Last().Add(new Vector2((float)addressList[0].Latitude, (float)addressList[0].Longtitude));
            // }

            // routes.Last().AddRange(route.Value.Shape.Select(s => new Vector2(s.Latitude, s.Longitude)));

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
                    routes.Last().Add(new Vector2((float)addressList[i].Latitude, (float)addressList[i].Longtitude));
                    routes.Last().Add(new Vector2((float)addressList[i + 1].Latitude, (float)addressList[i + 1].Longtitude));
                }
                else routes.Last().AddRange(route.Value.Shape.Select(s => new Vector2(s.Latitude, s.Longitude)));
            }
        }

        return routes;
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
