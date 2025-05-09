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

namespace ZAK.Services.ApplicationsManagerSerivce;

public class ApplicationsManagerService : IApplicationsManagerService
{
    private readonly ILogger<ApplicationsManagerService> _logger;
    private readonly IFileLoader _fileLoader;
    private readonly IApplicationsScrapper _applicationScrapper;

    private readonly IDao<Application, ZAK.Db.Models.ApplicationModel> _applicationsDataAccess;
    private readonly IDao<Address, AddressModel> _addressesDataAccess;

    public ApplicationsManagerService(
        IDao<Application, Db.Models.ApplicationModel> applicationsDataAccess,
        IDao<Address, AddressModel> addressesDataAccess,
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
        await _applicationsDataAccess.DeleteRange(oldApplications.Except(newApplications));

        _logger.LogInformation("Old applications deleted successfully!");
    }

    public async Task UpdateOldApplications(List<Application> newApplications)
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

    public async Task AddNewApplcations(List<Application> parsedApplications)
    {
        _logger.LogInformation("Adding new applications...");



        //Insert addresses with districts

        List<Address> parsedAddresses = parsedApplications.Select(app => new Address(app.address)).ToList();
        parsedAddresses.RemoveAll(address => address is null);

        List<Address> oldAddresses = (await _addressesDataAccess.GetAll()).ToList();

        List<Address> newAddresses = parsedAddresses.Except(oldAddresses, new AddressComparer()).ToList();

        await _addressesDataAccess.InsertRange(
            newAddresses,
            inputProcessQuery: (addresses, dbContext) =>
                {
                    List<District> districts = dbContext.Set<DistrictModel>().Select(d => new District(d)).ToList();

                    foreach (Address add in addresses)
                    {
                        District? districtFromDb = districts.Find(dist =>
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

                        // if (districtFromDb is not null)
                        // {
                        //     add.district = districtFromDb;
                        //     if(!dbContext.Set<DistrictModel>().Local.Any(dis => districtFromDb.name == dis.name )) 
                        //     {
                        //         dbContext.Attach(add.district);
                        //     }
                        // } else if(add.district is not null)
                        // {
                        //     dbContext.Add(add.district);
                        //     dbContext.SaveChanges();
                        //     districts = dbContext.Set<DistrictModel>().Select(d => new District(d)).ToList();

                        // }

                    }
                    return addresses;
                }
        );

        foreach (Application app in parsedApplications) if (app.address is not null) app.address.district = null;

        //Insert applications with addresses

        List<Application> oldApplications = (await _applicationsDataAccess.GetAll()).ToList();

        List<Application> newApplications = parsedApplications.Except(oldApplications).ToList();
        try
        {

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

        }
        catch (Exception ex)
        {

        }










        // newApplications.Except(oldApplications).ToList().ForEach(async (newApplication) =>
        // {
        //     await _applicationsDataAccess.Insert(newApplication, inputProcessQuery: (query, newApplication, dbContext) =>
        //     {
        //         AddressModel? possibleAddress = dbContext.
        //             Set<AddressModel>().AsTracking().
        //             FirstOrDefault(ad => ad.streetName == newApplication.address!.streetName && ad.building == newApplication.address.building);

        //         if (possibleAddress is not null)
        //         {
        //             newApplication.address = possibleAddress;
        //             newApplication.address.district = null;
        //             dbContext.Attach(newApplication.address);

        //         }
        //         return newApplication;
        //     });
        // });

        _logger.LogInformation("New applications added successfully!");
    }
}

class DistrictsComparer : IEqualityComparer<DistrictModel?>
{
    public bool Equals(DistrictModel? x, DistrictModel? y)
    {
        if (x is null || y is null) return false;
        if (x.name == y.name) return true;
        else return false;
    }

    public int GetHashCode([DisallowNull] DistrictModel obj)
    {
        return obj.name.GetHashCode();
    }
}

class AddressComparer : IEqualityComparer<Address?>
{
    public bool Equals(Address? x, Address? y)
    {
        if (x is null || y is null) return false;
        if (x.streetName == y.streetName && x.building == y.building) return true;
        else return false;
    }

    public int GetHashCode([DisallowNull] Address? obj)
    {
        return (obj.streetName + obj.building).GetHashCode();
    }
}

class ApplicationComparer : IEqualityComparer<Application?>
{
    public bool Equals(Application? x, Application? y)
    {
        if (x is null || y is null) return false;
        if (x.id == y.id) return true;
        else return false;
    }

    public int GetHashCode([DisallowNull] Application? obj)
    {
        throw new NotImplementedException();
    }
}
