using BlazorApp.DA;
using ApplicationsScrappingModule;
using BlazorApp.GeoDataManager;

namespace BlazorApp.ApplicationsLoader;

public class ApplicationsLoader : IApplicationsLoader
{
    IApplicationsScrapper _applicationScrapper;
    IApplicationsDataAccess _applicationsDataAccess;
    IGeoDataManager _geoDataManager;

    public ApplicationsLoader(IApplicationsScrapper applicationScrapper,    
                        IApplicationsDataAccess applicationsDataAccess, 
                        IGeoDataManager geoDataManager)
    {
        _applicationScrapper = applicationScrapper;
        _applicationsDataAccess = applicationsDataAccess;
        _geoDataManager = geoDataManager;
    }

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
    }
}
