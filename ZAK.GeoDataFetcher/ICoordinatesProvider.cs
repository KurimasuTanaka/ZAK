using System;
using ZAK.DA;

namespace BlazorApp.GeoDataManager;

public interface ICoordinatesProvider
{
    public Task GetCoordinatesForAddress(Address address);
}
