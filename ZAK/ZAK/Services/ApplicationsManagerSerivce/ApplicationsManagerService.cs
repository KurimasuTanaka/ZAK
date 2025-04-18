using System;
using ApplicationsScrappingModule;
using BlazorApp;
using BlazorApp.DA;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using ZAK.Da.BaseDAO;
using ZAK.Db.Models;

namespace ZAK.Services.ApplicationsManagerSerivce;

public class ApplicationsManagerService : IApplicationsManagerService
{
    private readonly ILogger<ApplicationsManagerService> _logger;
    private readonly IFileLoader _fileLoader;
    private readonly IApplicationsScrapper _applicationScrapper;

    private readonly IDaoBase<Application, ZAK.Db.Models.ApplicationModel> _applicationsDataAccess;
    private readonly IDaoBase<Address, AddressModel> _addressesDataAccess;

    public ApplicationsManagerService(
        IDaoBase<Application, ZAK.Db.Models.ApplicationModel> applicationsDataAccess,
        IDaoBase<Address, AddressModel> addressesDataAccess,
        IApplicationsScrapper applicationsScrapper,
        IFileLoader fileLoader,
        ILogger<ApplicationsManagerService> logger)
    {
        _fileLoader = fileLoader;
        _logger = logger;
        _applicationScrapper = applicationsScrapper;
        _applicationsDataAccess = applicationsDataAccess;
        _addressesDataAccess = addressesDataAccess;
    }


    public Task AddApplication()
    {
        throw new NotImplementedException();
    }

    public Task<List<Application>> GetApplications()
    {
        throw new NotImplementedException();
    }

    public async Task UpdateApplications(IBrowserFile file)
    {
        _logger.LogInformation("Updating applications...");

        await _fileLoader.LoadFile(file);

        List<Application> newApplications = await _applicationScrapper.ScrapApplicationData(_fileLoader.GetLastLoadedFile());

        //Updating application list in DB
        await DeleteOldApplications(newApplications);

        await UpdateOldApplications(newApplications);
        await AddNewApplcations(newApplications);

        _logger.LogInformation("Applications updated successfully!");
    }

    private async Task DeleteOldApplications(List<Application> newApplications)
    {
        _logger.LogInformation("Deleting old applications...");

        List<Application> oldApplications = (await _applicationsDataAccess.GetAll()).ToList();
        await _applicationsDataAccess.DeleteRange(oldApplications.Except(newApplications));

        _logger.LogInformation("Old applications deleted successfully!");
    }

    private async Task UpdateOldApplications(List<Application> newApplications)
    {
        _logger.LogInformation("Updating old applications...");

        List<Application> oldApplications = (await _applicationsDataAccess.GetAll()).ToList();

        foreach (Application application in newApplications.Intersect(oldApplications))
        {
            await _applicationsDataAccess.Update(
                application,
                application.id);
        }
        _logger.LogInformation("Old applications updated successfully!");
    }

    private async Task AddNewApplcations(List<Application> newApplications)
    {
        _logger.LogInformation("Adding new applications...");

        List<Application> oldApplications = (await _applicationsDataAccess.GetAll()).ToList();

        newApplications.Except(oldApplications).ToList().ForEach(async (newApplication) =>
        {
            await _applicationsDataAccess.Insert(newApplication, inputProcessQuery: (query, newApplication, dbContext) =>
            {

                AddressModel? possibleAddress = dbContext.
                    Set<AddressModel>().AsNoTracking().
                    FirstOrDefault(ad => ad.streetName == newApplication.address!.streetName && ad.building == newApplication.address.building);

                if (possibleAddress is not null) dbContext.Attach(newApplication.address);

                return newApplication;
            });
        });

        _logger.LogInformation("New applications added successfully!");
    }
}
