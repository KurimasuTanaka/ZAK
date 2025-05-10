using System;
using ApplicationsScrappingModule;
using BlazorApp;
using BlazorApp.DA;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using ZAK.DAO;
using ZAK.Db.Models;
using ZAK.Services.ApplicationsManagerSerivce;

namespace ZAK.Tests;
public class ApplicationsManagerTests : ZakTestBase
{
    [Fact]
    public async void UploadNewApplicationsToDb()
    {

        //Arrange
        IDao<Address, AddressModel> addressesDao = new Dao<Address, AddressModel>(dbContextFactory, addressDaoLogger);
        IDao<Application, ApplicationModel> applicationsDao = new Dao<Application, ApplicationModel>(dbContextFactory, applicationDaoLogger);
        IDao<District, DistrictModel> districtDao = new Dao<District, DistrictModel>(dbContextFactory, districtDaoLogger);

        ApplicationsManagerService applicationsManagerService = new(applicationsDao, addressesDao, null, null, applicationsManagerLogger);


        //Setup districts
        District district1 = new()
        {
            name = "Test district 1"
        };

        //Setup addresses
        Address address1 = new()
        {
            streetName = "Test street 1",
            building = "Test building 1",
            district = district1
        };
        Address address2 = new()
        {
            streetName = "Test street 2",
            building = "Test building 2",
            district = district1
        };

        //Setup applications
        Application application1 = new()
        {
            address = address1,
            operatorComment = "Application comment 1"
        };

        Application application2 = new()
        {
            address = address2,
            operatorComment = "Application comment 2"
        };

        List<Application> applicationsToInsert = new() { application1, application2 };

        //Act

        await applicationsManagerService.ProceedApplications(applicationsToInsert);

        //Assert
        List<Application> addedApplications = (await applicationsDao.GetAll()).ToList();
        List<Address> addedAddresses = (await addressesDao.GetAll()).ToList();
        List<District> addedDistricts = (await districtDao.GetAll()).ToList();

        Assert.Equal(2, addedAddresses.Count);
        Assert.Single(addedDistricts);
        Assert.Equal(2, addedApplications.Count);
    }


    [Fact]
    public async void UnploadNewApplicationsToDbWithExistingAddressAndApplications()
    {
        //Arrange
        IDao<Address, AddressModel> addressesDao = new Dao<Address, AddressModel>(dbContextFactory, addressDaoLogger);
        IDao<Application, ApplicationModel> applicationsDao = new Dao<Application, ApplicationModel>(dbContextFactory, applicationDaoLogger);
        IDao<District, DistrictModel> districtDao = new Dao<District, DistrictModel>(dbContextFactory, districtDaoLogger);

        ApplicationsManagerService applicationsManagerService = new(applicationsDao, addressesDao, null, null, applicationsManagerLogger);


        //Setup districts
        District district1 = new()
        {
            name = "Test district 1"
        };

        District district2 = new()
        {
            name = "Test district 2"
        };

        //Setup addresses
        Address address1 = new()
        {
            streetName = "Test street 1",
            building = "Test building 1",
            district = district1
        };
        Address address2 = new()
        {
            streetName = "Test street 2",
            building = "Test building 2",
            district = district2
        };
        Address address3 = new()
        {
            streetName = "Test street 3",
            building = "Test building 3",
            district = district2
        };

        //Setup applications
        Application application1 = new()
        {
            address = address1,
            operatorComment = "Application comment 1"
        };

        Application application2 = new()
        {
            address = address2,
            operatorComment = "Application comment 2"
        };

        Application application3 = new()
        {
            address = address2,
            operatorComment = "Application comment 3"
        };

        List<Application> applicationsToInsert = new() { application1, application2 };

        //Act

        await applicationsManagerService.ProceedApplications(applicationsToInsert);

        applicationsToInsert.Add(application3);
        applicationsToInsert[1].operatorComment = "Applications comment 2 UPDATED";
        applicationsToInsert.RemoveAt(0);

        await applicationsManagerService.ProceedApplications(applicationsToInsert);

        //Assert
        List<Application> addedApplications = (await applicationsDao.GetAll()).ToList();
        List<Address> addedAddresses = (await addressesDao.GetAll()).ToList();
        List<District> addedDistricts = (await districtDao.GetAll()).ToList();

        Assert.Equal(2, addedAddresses.Count);
        Assert.Equal(2, addedDistricts.Count);
        Assert.Equal(2, addedApplications.Count);
        Assert.Equal("Application comment 3", addedApplications.Last().operatorComment);
    }
}
