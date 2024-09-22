using BlazorApp.DA;
using ApplicationsScrappingModule;
using BlazorApp.GeoDataManager;

namespace BlazorApp.ApplicationsManager;

public class ApplicationsManager : IApplicationsManager
{
    IApplicationScrapper _applicationScrapper;
    IApplicationsDataAccess _applicationsDataAccess;
    IGeoDataManager _geoDataManager;

    public ApplicationsManager(IApplicationScrapper applicationScrapper,    
                        IApplicationsDataAccess applicationsDataAccess, 
                        IGeoDataManager geoDataManager)
    {
        _applicationScrapper = applicationScrapper;
        _applicationsDataAccess = applicationsDataAccess;
        _geoDataManager = geoDataManager;
    }

    public async Task AddNewApplication(Application application)
    {
        await _applicationsDataAccess.AddNewApplication(application);
    }

    public async Task AddNewApplications(string applicationsFilePath)
    {
        await _applicationsDataAccess.ClearApplicationsList();

        List<Application> newApplications = new List<Application>();

        _applicationScrapper.ScrapApplicationData(ref newApplications, applicationsFilePath);
        newApplications = await _geoDataManager.AddGeoDataToApplications(newApplications);

        _applicationsDataAccess.AddNewApplications(newApplications);

    }

    public async Task AddNewApplicationsWithRemoval(string applicationsFilePath)
    {
        await _applicationsDataAccess.ClearApplicationsList();

        List<Application> newApplications = new List<Application>();

        _applicationScrapper.ScrapApplicationData(ref newApplications, applicationsFilePath);
        newApplications = await _geoDataManager.AddGeoDataToApplications(newApplications);

        _applicationsDataAccess.AddApplications(newApplications);
    }

    public async Task<IEnumerable<Application>> GetApplications()
    {
        return _applicationsDataAccess.GetAllApplications();
    }

    public async Task<IEnumerable<Application>> GetApplicationsWithoutIgnored()
    {
        return _applicationsDataAccess.GetAllApplicationsWithIgnoring();
    }
}
