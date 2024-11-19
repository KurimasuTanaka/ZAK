using System;
using BlazorApp.DA;

namespace BlazorApp.GeoDataManager;

public interface ICoordinatesProvider
{
    public Task GetCoordinatesForAddress(Address address);
}
