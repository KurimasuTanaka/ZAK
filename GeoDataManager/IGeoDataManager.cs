namespace BlazorApp.GeoDataManager;

using BlazorApp.DA;

public interface IGeoDataManager
{
    public Task<List<Application>> AddGeoDataToApplications(List<Application> applications);
}
