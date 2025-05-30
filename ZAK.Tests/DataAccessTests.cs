using ZAK.DA;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using ZAK.DAO;
using ZAK.Db.Models;

namespace ZAK.Tests;

public class DataAccessTests : ZakTestBase
{

    [Fact]
    public async Task InsertNewApplicationToTheEmptyDb()
    {
        //Arrange

        //Act

        Address newAddress = new();
        newAddress.streetName = "PaperStreet";
        newAddress.building = "building";

        Application newApplication = new();
        newApplication.address = newAddress;

        await applicationRepository.CreateAsync(newApplication);

        //Assert
        Assert.Single(await addressRepository.GetAllAsync());
        Assert.Single(await applicationRepository.GetAllAsync());
    }

    [Fact]
    public async Task InsertNewAddressAndUpdateItWithoutTracking()
    {
        //Arrange

        //Act 
        Address address = new();
        address.streetName = "вулиця Володимира Івасюка";
        address.building = "54";

        await addressRepository.CreateAsync(address);

        Address addressToUpdate = (await addressRepository.GetAllAsync()).FirstOrDefault()!;
        addressToUpdate.streetName = "проспект Володимира Івасюка";
        await addressRepository.UpdateAsync(addressToUpdate);

        Address updatedAddress = (await addressRepository.GetAllAsync()).FirstOrDefault()!;

    

        //Assert
        Assert.NotNull(updatedAddress);
        Assert.Equal("проспект Володимира Івасюка", updatedAddress.streetName);
    }

    [Fact]
    public async Task InsertNewAddressAndUpdateRelatedEntityUsingInclueQuery()
    {
        //Arrange

        //Act 
        Address address = new();
        address.streetName = "проспект Володимира Івасюка";
        address.building = "54";

        await addressRepository.CreateAsync(address);

        Address addressToUpdate = (await addressRepository.GetAllAsync()).FirstOrDefault()!;
        addressToUpdate.coordinates = new AddressCoordinates();
        addressToUpdate.coordinates.lat = 5000;
        addressToUpdate.coordinates.lon = 5000;

        await addressRepository.UpdateAsync(addressToUpdate);

        Address updatedAddress = (await addressRepository.GetAllAsync()).FirstOrDefault()!;
        //Assert
        Assert.NotNull(updatedAddress);
        Assert.NotNull(updatedAddress.coordinates);
        Assert.Equal(5000, updatedAddress.coordinates.lat);
        Assert.Equal(5000, updatedAddress.coordinates.lon);
    }

    [Fact]
    public async Task InsertNewApplicationWithTheAlreadyExistedAddress()
    {
        //Arrange
        Address sharedAddress = new();
        sharedAddress.streetName = "PaperStreet 1";
        sharedAddress.building = "building";

        Application oldApplication = new();
        oldApplication.address = sharedAddress;

        await applicationRepository.CreateAsync(oldApplication);

        //Act

        Application newApplication = new();
        newApplication.address = sharedAddress;

        await applicationRepository.CreateAsync(newApplication);

        //Assert
        Assert.Equal(2, (await applicationRepository.GetAllAsync()).Count());
        Assert.Single(await addressRepository.GetAllAsync());
    }

    [Fact]
    public async Task InsertRangeOfApplicationsAndUpdateRangeOfApplicationsInBulk()
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

        await applicationRepository.CreateRangeAsync(applications);

        List<Application> applicationsFromDb = (await applicationRepository.GetAllAsync()).ToList();

        applicationsFromDb.RemoveAt(1);

        applicationsFromDb.ElementAt(0).operatorComment = "Comment 1 Edited";
        applicationsFromDb.ElementAt(1).operatorComment = "Comment 3 Edited";

        await applicationRepository.UpdateRangeAsync(
            applicationsFromDb);

        applicationsFromDb = (await applicationRepository.GetAllAsync()).ToList();

        //Assert
        Assert.Equal(3, applicationsFromDb.Count());
        Assert.Equal("Comment 1 Edited", applicationsFromDb.ElementAt(0).operatorComment);
        Assert.Equal("Comment 2 Unedited", applicationsFromDb.ElementAt(1).operatorComment);
        Assert.Equal("Comment 3 Edited", applicationsFromDb.ElementAt(2).operatorComment);
    }

    [Fact]
    public async Task InsertNewAddressesWithInsertRange()
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

        await addressRepository.CreateRangeAsync(addresses);

        //Assert

        List<Address> addressesFromDb = (await addressRepository.GetAllAsync()).ToList();
        List<District> districtsFromDb = (await districtsDao.GetAll()).ToList();

        Assert.Equal(3, addressesFromDb.Count);
        Assert.Equal(2, districtsFromDb.Count);
    }

}
