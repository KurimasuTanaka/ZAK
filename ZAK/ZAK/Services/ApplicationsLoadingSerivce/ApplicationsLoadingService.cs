using System;
using System.Diagnostics.CodeAnalysis;
using ApplicationsScrappingModule;
using BlazorApp;
using ZAK.DA;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using ZAK.DAO;
using ZAK.Db.Models;

namespace ZAK.Services.ApplicationsLoadingService;

public class ApplicationsLoadingService : IApplicationsLoadingService
{
    private readonly ILogger<ApplicationsLoadingService> _logger;

    // File loader and scrapper are optional dependencies for testing purposes.
    private readonly IFileLoader? _fileLoader;
    private readonly IApplicationsScrapper? _applicationScrapper;

    private readonly IApplicationReporisory _applicationRepository;
    private readonly IAddressRepository _addressRepository;

    public ApplicationsLoadingService(
        IApplicationReporisory applicationReporisory,
        IAddressRepository addressRepository,
        IApplicationsScrapper? applicationsScrapper,
        IFileLoader? fileLoader,
        ILogger<ApplicationsLoadingService> logger)
    {
        _fileLoader = fileLoader;
        _logger = logger;
        _applicationScrapper = applicationsScrapper;
        _applicationRepository = applicationReporisory;
        _addressRepository = addressRepository;
    }

    public async Task UpdateApplications(IBrowserFile file)
    {
        _logger.LogInformation("Updating applications...");

        await _fileLoader!.LoadFile(file);

        List<Application> parsedApplications = await _applicationScrapper!.ScrapApplicationData(_fileLoader.GetLastLoadedFile());

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

        List<Application> oldApplications = (await _applicationRepository.GetAllAsync()).ToList();

        List<Application> applicationsToDelete = oldApplications.
            Except(newApplications, new ApplicationComparer()).Where(a => a.userCreated is false).ToList();

        await _applicationRepository.DeleteRangeAsync(applicationsToDelete);

        _logger.LogInformation("Old applications deleted successfully!");
    }

    public async Task UpdateOldApplications(List<Application> newApplications)
    {
        _logger.LogInformation("Updating old applications...");

        List<Application> oldApplications = (await _applicationRepository.GetAllAsync()).ToList();

        List<Application> applicationsToUpdate = newApplications.Intersect(oldApplications, new ApplicationComparer()).ToList();

        for (int i = 0; i < applicationsToUpdate.Count; i++)
        {
            Application oldApp = oldApplications.Find(app => app.id == applicationsToUpdate[i].id)!;

            if (oldApp.address is not null && applicationsToUpdate[i].address is not null)
            {
                if (!(new AddressComparer().Equals(new Address(oldApp.address), new Address(applicationsToUpdate[i].address!)))) applicationsToUpdate[i].addresWasUpdated = true;
                
            }
            if (applicationsToUpdate[i].operatorComment != oldApp.operatorComment) applicationsToUpdate[i].operatorCommentWasUpdated = true;
            if (applicationsToUpdate[i].masterComment != oldApp.masterComment) applicationsToUpdate[i].masterCommentWasUpdated = true;
            if (applicationsToUpdate[i].stretchingStatus != oldApp.stretchingStatus) applicationsToUpdate[i].statusWasUpdated = true;

            applicationsToUpdate[i].ignored = oldApp.ignored;
            applicationsToUpdate[i].buried = oldApp.buried;
            applicationsToUpdate[i].office = oldApp.office;
            applicationsToUpdate[i].tarChangeApp = oldApp.tarChangeApp;
            applicationsToUpdate[i].freeCable = oldApp.freeCable;
            applicationsToUpdate[i].important = oldApp.important;
            applicationsToUpdate[i].maxDaysForConnection = oldApp.maxDaysForConnection;
            applicationsToUpdate[i].timeRangeIsSet = oldApp.timeRangeIsSet;
            applicationsToUpdate[i].secondPart = oldApp.secondPart;
            applicationsToUpdate[i].firstPart = oldApp.firstPart;
            applicationsToUpdate[i].startHour = oldApp.startHour;
            applicationsToUpdate[i].endHour = oldApp.endHour;
            applicationsToUpdate[i].statusWasChecked = oldApp.statusWasChecked;
            applicationsToUpdate[i].urgent = oldApp.urgent;
        }
        await _applicationRepository.UpdateRangeAsync(applicationsToUpdate);
        _logger.LogInformation("Old applications updated successfully!");
    }

    public async Task AddNewApplcations(List<Application> parsedApplications)
    {
        _logger.LogInformation("Adding new applications...");

        //Insert addresses with districts

        List<Address> parsedAddresses = parsedApplications.Where(app => app.address is not null).Select(app => new Address(app.address!)).ToList();
        parsedAddresses.RemoveAll(address => address is null);

        List<Address> oldAddresses = (await _addressRepository.GetAllAsync()).ToList();

        List<Address> newAddresses = new();
        if (oldAddresses.Count() is not 0) newAddresses = parsedAddresses.Except(oldAddresses, new AddressComparer()).ToList();
        else newAddresses = parsedAddresses;

        await _addressRepository.CreateRangeAsync(newAddresses);

        foreach (Application app in parsedApplications) if (app.address is not null) app.address.district = null;

        //Insert applications with addresses

        List<Application> oldApplications = (await _applicationRepository.GetAllAsync()).ToList();

        List<Application> newApplications = new List<Application>();
        if (oldApplications.Count() is not 0) newApplications = parsedApplications.Except(oldApplications, new ApplicationComparer()).ToList();
        else newApplications = parsedApplications;

        await _applicationRepository.CreateRangeAsync(newApplications);

        _logger.LogInformation("New applications added successfully!");
    }
}



