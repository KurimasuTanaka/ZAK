using System;
using System.Diagnostics.CodeAnalysis;
using ApplicationsScrappingModule;
using BlazorApp;
using BlazorApp.DA;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using ZAK.DAO;
using ZAK.Db.Models;

namespace ZAK.Services.ApplicationsLoadingService;

public class ApplicationsLoadingService : IApplicationsLoadingService
{
    private readonly ILogger<ApplicationsLoadingService> _logger;
    private readonly IFileLoader _fileLoader;
    private readonly IApplicationsScrapper _applicationScrapper;

    private readonly IDao<Application, ZAK.Db.Models.ApplicationModel> _applicationsDataAccess;
    private readonly IDao<Address, AddressModel> _addressesDataAccess;

    public ApplicationsLoadingService(
        IDao<Application, Db.Models.ApplicationModel> applicationsDataAccess,
        IDao<Address, AddressModel> addressesDataAccess,
        IApplicationsScrapper applicationsScrapper,
        IFileLoader fileLoader,
        ILogger<ApplicationsLoadingService> logger)
    {
        _fileLoader = fileLoader;
        _logger = logger;
        _applicationScrapper = applicationsScrapper;
        _applicationsDataAccess = applicationsDataAccess;
        _addressesDataAccess = addressesDataAccess;
    }

    public async Task UpdateApplications(IBrowserFile file)
    {
        _logger.LogInformation("Updating applications...");

        await _fileLoader.LoadFile(file);

        List<Application> parsedApplications = await _applicationScrapper.ScrapApplicationData(_fileLoader.GetLastLoadedFile());

        //Updating application list in DB
        await ProceedApplications(parsedApplications);

        _logger.LogInformation("Applications updated successfully!");
    }

    public async Task ProceedApplications(List<Application> parsedApplications)
    {
        await DeleteOldApplications(parsedApplications);

        await UpdateOldApplications(parsedApplications);
        await AddNewApplcations(parsedApplications);

    }

    public async Task DeleteOldApplications(List<Application> newApplications)
    {
        _logger.LogInformation("Deleting old applications...");

        List<Application> oldApplications = (await _applicationsDataAccess.GetAll()).ToList();

        List<Application> applicationsToDelete = oldApplications.Except(newApplications, new ApplicationComparer()).ToList();

        await _applicationsDataAccess.DeleteRange(applicationsToDelete);

        _logger.LogInformation("Old applications deleted successfully!");
    }

    public async Task UpdateOldApplications(List<Application> newApplications)
    {
        _logger.LogInformation("Updating old applications...");

        List<Application> oldApplications = (await _applicationsDataAccess.GetAll()).ToList();

        List<Application> applicationsToUpdate = newApplications.Intersect(oldApplications, new ApplicationComparer()).ToList();
        for (int i = 0; i < applicationsToUpdate.Count; i++)
        {
            Application newApp = newApplications.Find(app => app.id == applicationsToUpdate[i].id)!;

            if(applicationsToUpdate[i].address != newApp.address) applicationsToUpdate[i].addresWasUpdated = true;      
            if(applicationsToUpdate[i].operatorComment != newApp.operatorComment) applicationsToUpdate[i].operatorCommentWasUpdated = true;      
            if(applicationsToUpdate[i].masterComment != newApp.masterComment) applicationsToUpdate[i].masterCommentWasUpdated = true;      
            if(applicationsToUpdate[i].stretchingStatus != newApp.stretchingStatus) applicationsToUpdate[i].statusWasUpdated = true;      
            
        }
        await _applicationsDataAccess.UpdateRange(applicationsToUpdate);
        _logger.LogInformation("Old applications updated successfully!");
    }

    public async Task AddNewApplcations(List<Application> parsedApplications)
    {
        _logger.LogInformation("Adding new applications...");

        //Insert addresses with districts

        List<Address> parsedAddresses = parsedApplications.Select(app => new Address(app.address)).ToList();
        parsedAddresses.RemoveAll(address => address is null);

        List<Address> oldAddresses = (await _addressesDataAccess.GetAll()).ToList();

        List<Address> newAddresses = new();
        if(oldAddresses.Count() is not 0) newAddresses = parsedAddresses.Except(oldAddresses, new AddressComparer()).ToList();
        else newAddresses = parsedAddresses;

        await _addressesDataAccess.InsertRange(
            newAddresses,
            inputProcessQuery: (addresses, dbContext) =>
                {
                    List<BlazorApp.DA.District> districts = dbContext.Set<Db.Models.DistrictModel>().Select(d => new BlazorApp.DA.District(d)).ToList();

                    foreach (Address add in addresses)
                    {
                        BlazorApp.DA.District? districtFromDb = districts.Find(dist =>
                        {
                            if (add.district is not null && dist.name == add.district.name)
                            {
                                return true;
                            }
                            return false;
                        });

                        if (districtFromDb is not null)
                        {
                            add.district = districtFromDb;
                            dbContext.Attach(add.district);
                        }


                    }
                    return addresses;
                }
        );

        foreach (Application app in parsedApplications) if (app.address is not null) app.address.district = null;

        //Insert applications with addresses

        List<Application> oldApplications = (await _applicationsDataAccess.GetAll()).ToList();

        List<Application> newApplications = new List<Application>();
        if (oldApplications.Count() is not 0) newApplications = parsedApplications.Except(oldApplications, new ApplicationComparer()).ToList();
        else newApplications = parsedApplications;

        await _applicationsDataAccess.InsertRange(
            newApplications,
            inputProcessQuery: (applications, dbContext) =>
            {
                List<Address> addresses = dbContext.Set<AddressModel>().Select(a => new Address(a)).ToList();

                foreach (Application app in applications)
                {
                    Address? addressFromDb = addresses.Find(add =>
                    {
                        if (app.address.streetName == add.streetName && app.address.building == add.building)
                        {
                            return true;
                        }
                        return false;
                    });

                    if (addressFromDb is not null)
                    {
                        app.address = addressFromDb;
                        dbContext.Attach(app.address);
                    }

                }
                return applications;
            }
        );

        _logger.LogInformation("New applications added successfully!");
    }
}



