using System;
using BlazorApp.DA;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using ZAK.Da.BaseDAO;
using ZAK.Db.Models;
using ZAK.Services.ApplicationsManagerSerivce;

namespace ZAK.Tests;
[Collection("Non-Parallel Collection")]
public class ApplicationsManagerTests
{
    [Fact]
    public async void UploadNewApplicationsToDb()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<ApplicationsManagerService> applicationsManagerLogger = new NullLogger<ApplicationsManagerService>();
        ILogger<DaoBase<Address, AddressModel>> addressDaoLogger = new NullLogger<DaoBase<Address, AddressModel>>();
        ILogger<DaoBase<Application, ApplicationModel>> applicationDaoLogger = new NullLogger<DaoBase<Application, ApplicationModel>>();
        ILogger<DaoBase<District, DistrictModel>> districtDaoLogger = new NullLogger<DaoBase<District, DistrictModel>>();

        IDaoBase<Address, AddressModel> addressesDao = new DaoBase<Address, AddressModel>(dbContextFactory, addressDaoLogger);
        IDaoBase<Application, ApplicationModel> applicationsDao = new DaoBase<Application, ApplicationModel>(dbContextFactory, applicationDaoLogger);
        IDaoBase<District, DistrictModel> districtDao = new DaoBase<District, DistrictModel>(dbContextFactory, districtDaoLogger);

        ApplicationsManagerService applicationsManagerService = new(applicationsDao, addressesDao, null, null, applicationsManagerLogger);

        //Act

        Application application1 = new()
        {
            address = new AddressModel()
            {
                streetName = "paper st 1",
                district = new DistrictModel()
                {
                    name = "DIST1"
                }
            },
            operatorComment = "test comment 1"
        };
        Application application2 = new()
        {
            address = new AddressModel()
            {
                streetName = "paper st 2",
                district = new DistrictModel()
                {
                    name = "DIST2"
                }
            },
            operatorComment = "test comment 2"
        };
        Application application3 = new()
        {
            address = new AddressModel()
            {
                streetName = "paper st 1",
                district = new DistrictModel()
                {
                    name = "DIST1"
                }
            },
            operatorComment = "test comment 3"
        };

        List<Application> applications = new List<Application>() { application1, application2, application3 };

        await applicationsManagerService.AddNewApplcations(applications);

        await applicationsManagerService.DeleteOldApplications(applications);
        await applicationsManagerService.UpdateOldApplications(applications);
        await applicationsManagerService.AddNewApplcations(applications);


        //Assert
        List<Application> addedApplications = (await applicationsDao.GetAll()).ToList();
        List<Address> addedAddresses = (await addressesDao.GetAll()).ToList();
        List<District> addedDistricts = (await districtDao.GetAll()).ToList();

        Assert.Equal(2, addedAddresses.Count);
        Assert.Equal(2, addedDistricts.Count);
        Assert.Equal(3, addedApplications.Count);

        dbContextFactory.DeleteTestDb();
    }

}
