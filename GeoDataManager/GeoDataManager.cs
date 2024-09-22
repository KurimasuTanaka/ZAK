using BlazorApp.DA;

namespace BlazorApp.GeoDataManager;

public class GeoDataManager : IGeoDataManager
{
    public async Task<List<Application>> AddGeoDataToApplications(List<Application> applications)
    {
        return applications;
    }
}
