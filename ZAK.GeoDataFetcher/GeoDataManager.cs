using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using BlazorApp.DA;
using Microsoft.EntityFrameworkCore;
using ZAK.Da.BaseDAO;
using ZAK.Db.Models;

namespace BlazorApp.GeoDataManager;


public class GeoDataManager : IGeoDataManager
{
    IDaoBase<Address, AddressModel> _addressesDataAccess;
    ICoordinatesProvider? _coordinatesProvider = new NominatimCoordinatesProvider();


    public GeoDataManager(IDaoBase<Address, AddressModel> addressesDataAccess)
    {
        _addressesDataAccess = addressesDataAccess;
    }

    public async Task PopulateApplicationsWithGeoData()
    {
        List<Address> addresses = (await _addressesDataAccess.GetAll(
            query: a => a.Include(ad => ad.coordinates).Include(ad => ad.addressAlias)
        )).Where(a => a.coordinates is null || (a.coordinates.lat == 0 || a.coordinates.lon == 0)).Take(10).ToList();

        foreach (Address address in addresses)
        {
            if (address.coordinates is null) address.coordinates = new AddressCoordinates();
            await _coordinatesProvider!.GetCoordinatesForAddress(address);
        }

        await _addressesDataAccess.UpdateRange(
            addresses,
            findPredicate: a =>
            {
                foreach (Address ad in addresses) if (a.Id == ad.Id) return true;
                return false;
            },
            includeQuery: (baseQuery) =>
            {
                return baseQuery.Include(a => a.coordinates);

            },
            updatingFunction: (oldAddress, newAddress) =>
            {
                if (oldAddress.coordinates is null) oldAddress.coordinates = newAddress.coordinates;
                else
                {
                    oldAddress.coordinates.lat = newAddress.coordinates!.lat;
                    oldAddress.coordinates.lon = newAddress.coordinates!.lon;
                }
                return oldAddress;
            },
            enitySeach: (entityArray, oldEntity) =>
            {
                return entityArray.FirstOrDefault(e => e.Id == oldEntity.Id)!;
            },
            attachFunction: (context, entity) =>
            {
                if (entity.coordinates is not null) context.Entry(entity).State = EntityState.Modified;
                return entity;
            }
        );
    }

}
