using BlazorApp.DA;
using ApplicationsScrappingModule;

namespace BlazorApp.ApplicationsLoader;

public class ApplicationsLoader(IApplicationsScrapper applicationScrapper, IApplicationsDataAccess applicationsDataAccess) : IApplicationsLoader
{
    IApplicationsScrapper _applicationScrapper = applicationScrapper;
    IApplicationsDataAccess _applicationsDataAccess = applicationsDataAccess;

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
