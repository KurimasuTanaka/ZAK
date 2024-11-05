namespace BlazorApp.GeoDataManager;

using BlazorApp.DA;

public interface IGeoDataManager
{
    public Task PopulateApplicationsWithGeoData();
}
