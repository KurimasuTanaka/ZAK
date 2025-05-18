using ZAK.DA;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ZAK.Services.ApplicationsLoadingService;

namespace ZAK.Tests;
public class ApplicationsLoadingTests : ZakTestBase
{
    [Fact]
    public async void UploadNewApplicationsToDb()
    {
        //Arrange

        ApplicationsLoadingService applicationsManagerService = new(applicationsDao, addressesDao, null, null, applicationsManagerLogger);


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
        List<District> addedDistricts = (await districtsDao.GetAll()).ToList();

        Assert.Equal(2, addedAddresses.Count);
        Assert.Single(addedDistricts);
        Assert.Equal(2, addedApplications.Count);
    }


    [Fact]
    public async void UnploadNewApplicationsToDbWithExistingAddressAndApplications()
    {
        //Arrange
        ApplicationsLoadingService applicationsManagerService = new(applicationsDao, addressesDao, null, null, applicationsManagerLogger);


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

        applicationsToInsert[1].statusWasChecked = true;
        Application applicationToUpdate = applicationsToInsert[1];
        applicationToUpdate.address = null;
        await applicationsDao.Update(applicationToUpdate, findPredicate: a => a.id == applicationToUpdate.id);

        applicationsToInsert.Add(application3);
        applicationsToInsert[1].operatorComment = "Applications comment 2 UPDATED";
        applicationsToInsert[1].address = address3;
        applicationsToInsert.RemoveAt(0);

        await applicationsManagerService.ProceedApplications(applicationsToInsert);

        //Assert
        List<Application> addedApplications = (await applicationsDao.GetAll(
            query => query.Include(a => a.address)
        )).ToList();

        List<Address> addedAddresses = (await addressesDao.GetAll()).ToList();
        List<District> addedDistricts = (await districtsDao.GetAll()).ToList();

        Assert.Equal(3, addedAddresses.Count);
        Assert.Equal(address3.building, addedApplications.Where(a => a.operatorComment == "Applications comment 2 UPDATED").First().address!.building);
        Assert.True(addedApplications.Where(a => a.operatorComment == "Applications comment 2 UPDATED").First().statusWasChecked);
        Assert.Equal(2, addedDistricts.Count);
        Assert.Equal(2, addedApplications.Count);
        Assert.Single(addedApplications.Where(a => a.applicationWasUpdated is true));
        Assert.Equal("Application comment 3", addedApplications.Last().operatorComment);
    }
}
