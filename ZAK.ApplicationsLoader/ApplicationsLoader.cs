using BlazorApp.DA;
using ApplicationsScrappingModule;
using BlazorApp.GeoDataManager;

namespace BlazorApp.ApplicationsLoader;

public class ApplicationsLoader(IApplicationsScrapper applicationScrapper, IApplicationsDataAccess applicationsDataAccess, IGeoDataManager geoDataManager) : IApplicationsLoader
{
    IApplicationsScrapper _applicationScrapper = applicationScrapper;
    IApplicationsDataAccess _applicationsDataAccess = applicationsDataAccess;
    IGeoDataManager _geoDataManager = geoDataManager;

    public async Task AddNewApplications(string applicationsFilePath)
    {
        await _applicationsDataAccess.ClearApplicationsList();

        List<Application> newApplications = await _applicationScrapper.ScrapApplicationData(applicationsFilePath);

        await _applicationsDataAccess.AddNewApplications(newApplications);

    }

    public async Task AddNewApplicationsWithRemoval(string applicationsFilePath)
    {
        await _applicationsDataAccess.ClearApplicationsList();

        List<Application> newApplications = await _applicationScrapper.ScrapApplicationData(applicationsFilePath);

        await _applicationsDataAccess.AddApplications(newApplications);
    
        await _geoDataManager.PopulateApplicationsWithGeoData();
    }
}
