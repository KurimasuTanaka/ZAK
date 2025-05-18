using BlazorApp.DA;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Abstractions;
using ZAK.DAO;
using ZAK.Db.Models;

namespace ZAK.Tests;

public class DAOTests : ZakTestBase
{

    [Fact]
    public async void InsertNewApplicationToTheEmptyDb()
    {
        //Arrange

        //Act

        Address newAddress = new();
        newAddress.streetName = "PaperStreet";
        newAddress.building = "building";

        Application newApplication = new();
        newApplication.address = newAddress;

        await applicationsDao.Insert(newApplication);

        //Assert
        Assert.Single(await addressesDao.GetAll());
        Assert.Single(await applicationsDao.GetAll());
    }

    [Fact]
    public async void InsertNewAddressAndUpdateItWithoutTracking()
    {
        //Arrange

        //Act 
        Address address = new();
        address.streetName = "вулиця Володимира Івасюка";
        address.building = "54";

        await addressesDao.Insert(address);

        Address addressToUpdate = (await addressesDao.GetAll()).FirstOrDefault()!;
        addressToUpdate.streetName = "проспект Володимира Івасюка";
        await addressesDao.Update(addressToUpdate, findPredicate: q => q.Id == addressToUpdate.Id);

        Address updatedAddress = (await addressesDao.GetAll()).FirstOrDefault()!;

        //Assert
        Assert.NotNull(updatedAddress);
        Assert.Equal("проспект Володимира Івасюка", updatedAddress.streetName);
    }

    [Fact]
    public async void InsertNewAddressAndUpdateRelatedEntityUsingInclueQuery()
    {
        //Arrange

        //Act 
        Address address = new();
        address.streetName = "проспект Володимира Івасюка";
        address.building = "54";

        await addressesDao.Insert(address);

        Address addressToUpdate = (await addressesDao.GetAll()).FirstOrDefault()!;
        addressToUpdate.coordinates = new AddressCoordinates();
        addressToUpdate.coordinates.lat = 5000;
        addressToUpdate.coordinates.lon = 5000;

        await addressesDao.Update(
            addressToUpdate,
            addressToUpdate.Id, 
            includeQuery: q => 
            {
                return q.Include(e => e.coordinates);
            },
            findPredicate: q =>
            {
                return q.Id == addressToUpdate.Id;
            }
        );

        Address updatedAddress = (await addressesDao.GetAll(
            query: q => {
                return q.Include(e => e.coordinates);
            }
        )).FirstOrDefault()!;

        //Assert
        Assert.NotNull(updatedAddress);
        Assert.NotNull(updatedAddress.coordinates);
        Assert.Equal(5000, updatedAddress.coordinates.lat);
        Assert.Equal(5000, updatedAddress.coordinates.lon);
    }

    [Fact]
    public async void InsertNewApplicationWithTheAlreadyExistedAddress()
    {
        //Arrange
        Address sharedAddress = new();
        sharedAddress.streetName = "PaperStreet 1";
        sharedAddress.building = "building";

        Application oldApplication = new();
        oldApplication.address = sharedAddress;

        await applicationsDao.Insert(oldApplication);

        //Act

        Application newApplication = new();
        newApplication.address = sharedAddress;

        Address sharedAddressToEdit = (await addressesDao.GetAll(query: a => a.Include(a => a.applications))).FirstOrDefault()!;


        await applicationsDao.Insert(newApplication,
            inputProcessQuery: (query, newApplication, dbContext) =>
            {
                dbContext.Attach(newApplication.address);
                return newApplication;
            }
        );

        //Assert
        Assert.Equal(2, (await applicationsDao.GetAll()).Count());
        Assert.Single(await addressesDao.GetAll());
    }

    [Fact]
    public async void InsertRangeOfApplicationsAndUpdateRangeOfApplicationsInBulk()
    {
        //Arrange
        Application application1 = new Application();
        application1.operatorComment = "Comment 1 Unedited";

        Application application2 = new Application();
        application2.operatorComment = "Comment 2 Unedited";

        Application application3 = new Application();
        application3.operatorComment = "Comment 3 Unedited";

        //Act

        List<Application> applications = new([application1, application2, application3]);

        await applicationsDao.InsertRange(applications);

        List<Application> applicationsFromDb = (await applicationsDao.GetAll()).ToList();

        applicationsFromDb.RemoveAt(1);

        applicationsFromDb.ElementAt(0).operatorComment = "Comment 1 Edited";
        applicationsFromDb.ElementAt(1).operatorComment = "Comment 3 Edited";

        await applicationsDao.UpdateRange(
            applicationsFromDb,
            findPredicate: a =>
            {
                foreach (Application ad in applicationsFromDb) if (a.id == ad.id) return true;
                return false;
            },
            updatingFunction: (oldApplication, newApplication) =>
            {
                oldApplication.operatorComment = newApplication.operatorComment;
                return oldApplication;
            },
            enitySeach: (entityArray, oldEntity) =>
            {
                return entityArray.FirstOrDefault(e => e.id == oldEntity.id)!;
            });

        applicationsFromDb = (await applicationsDao.GetAll()).ToList();

        //Assert
        Assert.Equal(3, applicationsFromDb.Count());
        Assert.Equal("Comment 1 Edited", applicationsFromDb.ElementAt(0).operatorComment);
        Assert.Equal("Comment 2 Unedited", applicationsFromDb.ElementAt(1).operatorComment);
        Assert.Equal("Comment 3 Edited", applicationsFromDb.ElementAt(2).operatorComment);
    }

    [Fact]
    public async void InsertNewAddressesWithInsertRange()
    {
        //Arrange

        //Act 
        District district1 = new();
        district1.name = "DIST1";

        District district2 = new();
        district2.name = "DIST2";

        Address address1 = new();
        address1.streetName = "проспект Володимира Івасюка";
        address1.building = "54";
        address1.district = district1;

        Address address2 = new();
        address2.streetName = "вулиця Богатирська";
        address2.building = "6-1";
        address2.district = district2;

        Address address3 = new();
        address3.streetName = "вулиця Прирічна";
        address3.building = "25";
        address3.district = district1;

        List<Address> addresses = new () {address1, address2, address3};

        await addressesDao.InsertRange(addresses);

        //Assert

        List<Address> addressesFromDb = (await addressesDao.GetAll()).ToList();
        List<District> districtsFromDb = (await districtsDao.GetAll()).ToList();

        Assert.Equal(3, addressesFromDb.Count);
        Assert.Equal(2, districtsFromDb.Count);
    }

}
