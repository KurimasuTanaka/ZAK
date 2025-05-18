using ZAK.DA;
using ApplicationsScrappingModule;
using ZAK.DAO;
using ZAK.Db.Models;
using Microsoft.Extensions.Logging;

namespace BlazorApp.ApplicationsLoader;

public class ApplicationsLoader : IApplicationsLoader
{
    IApplicationsScrapper _applicationScrapper;
    IDao<Application, ApplicationModel> _applicationsDataAccess;
    ILogger<ApplicationsLoader> _logger;


    public ApplicationsLoader(ILogger<ApplicationsLoader> logger, IDao<Application, ApplicationModel> applicationsDataAccess, IApplicationsScrapper applicationScrapper)
    {
        _applicationsDataAccess = applicationsDataAccess;
        _applicationScrapper = applicationScrapper;
        _logger = logger;
    }

    public async Task AddNewApplications(string applicationsFilePath)
    {
        _logger.LogInformation($"Adding new applications from file: {applicationsFilePath} ...");

        _logger.LogInformation("Starting parsing of applications file...");
        List<Application> newApplications = await _applicationScrapper.ScrapApplicationData(applicationsFilePath);

        await _applicationsDataAccess.InsertRange(newApplications);

        _logger.LogInformation("Applications added successfully.");
    }

    public async Task AddNewApplicationsWithRemoval(string applicationsFilePath)
    {
        _logger.LogInformation($"Adding new applications from file: {applicationsFilePath} with removal...");

        _logger.LogInformation("Removing all applications...");
        await _applicationsDataAccess.DeleteAll();

        _logger.LogInformation("Starting parsing of applications file...");
        List<Application> newApplications = await _applicationScrapper.ScrapApplicationData(applicationsFilePath);

        _logger.LogInformation("Inserting new applications...");
        await _applicationsDataAccess.InsertRange(newApplications);

        _logger.LogInformation("Applications added successfully.");
    }
}
