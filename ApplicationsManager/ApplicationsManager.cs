using BlazorApp.DA;
using ApplicationsScrappingModule;
using BlazorApp.GeoDataManager;

namespace BlazorApp.ApplicationsManager;

public class ApplicationsManager : IApplicationsManager
{
    IApplicationsScrapper _applicationScrapper;
    IApplicationsDataAccess _applicationsDataAccess;
    IGeoDataManager _geoDataManager;

    public ApplicationsManager(IApplicationsScrapper applicationScrapper,    
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

        List<Application> newApplications = await _applicationScrapper.ScrapApplicationData(applicationsFilePath);

        await _applicationsDataAccess.AddNewApplications(newApplications);

    }

    public async Task AddNewApplicationsWithRemoval(string applicationsFilePath)
    {
        await _applicationsDataAccess.ClearApplicationsList();

        List<Application> newApplications = await _applicationScrapper.ScrapApplicationData(applicationsFilePath);

        await _applicationsDataAccess.AddApplications(newApplications);
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
