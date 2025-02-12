using BlazorApp.DA;
using ApplicationsScrappingModule;
using ZAK.Da.BaseDAO;
using ZAK.Db.Models;

namespace BlazorApp.ApplicationsLoader;

public class ApplicationsLoader : IApplicationsLoader
{
    IApplicationsScrapper _applicationScrapper;
    IDaoBase<Application, ApplicationModel> _applicationsDataAccess;


    public ApplicationsLoader(IDaoBase<Application, ApplicationModel> applicationsDataAccess, IApplicationsScrapper applicationScrapper)
    {
        _applicationsDataAccess = applicationsDataAccess;
        _applicationScrapper = applicationScrapper;
    }

    public async Task AddNewApplications(string applicationsFilePath)
    {
        List<Application> newApplications = await _applicationScrapper.ScrapApplicationData(applicationsFilePath);

        await _applicationsDataAccess.InsertRange(newApplications);
    }

    public async Task AddNewApplicationsWithRemoval(string applicationsFilePath)
    {
        await _applicationsDataAccess.DeleteAll();

        List<Application> newApplications = await _applicationScrapper.ScrapApplicationData(applicationsFilePath);

        await _applicationsDataAccess.InsertRange(newApplications);
    }
}
