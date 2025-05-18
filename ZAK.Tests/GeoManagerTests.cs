using ZAK.DA;
using ZAK.Db.Models;
using BlazorApp.GeoDataManager;
using Xunit;
using Microsoft.Extensions.Logging;
using ZAK.DAO;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ZAK.Tests;

public class GeoDataManagerTests : ZakTestBase
{
    [Fact]
    public async void CoordinatesLoadingTest()
    {
        //Arrange
        IDao<Application, ApplicationModel> applicationsDAO = new Dao<Application, ApplicationModel>(dbContextFactory, applicationsDaoLogger);

        Address address1 = new();
        address1.streetName = "вулиця Володимира Івасюка";
        address1.building = "54";

        Address address2 = new();
        address2.streetName = "вулиця Володимира Івасюка";
        address2.building = "55";

        Address address3 = new();
        address3.streetName = "вулиця Богатирська";
        address3.building = "6-1";

        await addressRepository.CreateAsync(address1);
        await addressRepository.CreateAsync(address2);
        await addressRepository.CreateAsync(address3);

        GeoDataManager geoDataManager = new(addressRepository);

        //Act 

        await geoDataManager.PopulateApplicationsWithGeoData();

        //Assert

        List<Address> populatedAddresses = (await addressRepository.GetAllAsync()).ToList();

        Assert.NotNull(populatedAddresses[0].coordinates);
        Assert.NotNull(populatedAddresses[1].coordinates);
        Assert.NotNull(populatedAddresses[2].coordinates);
    }

    [Fact]
    public async void UpdatingExistedCoordinates()
    {
        //Arrange
        IDao<Application, ApplicationModel> applicationsDAO = new Dao<Application, ApplicationModel>(dbContextFactory, applicationsDaoLogger);

        Address address = new();
        address.streetName = "вулиця Володимира Івасюка";
        address.building = "54";

        await addressRepository.CreateAsync(address);

        GeoDataManager geoDataManager = new(addressRepository);

        //Act 
        await geoDataManager.PopulateApplicationsWithGeoData();

        Address addressToUpdate = (await addressRepository.GetAllAsync()).FirstOrDefault()!;
        addressToUpdate.streetName = "проспект Володимира Івасюка";
        await addressRepository.UpdateAsync(addressToUpdate);

        Address addressToUpdate2 = (await addressRepository.GetAllAsync()).FirstOrDefault()!;


        await geoDataManager.PopulateApplicationsWithGeoData();

        //Assert

        List<Address> populatedAddresses = (await addressRepository.GetAllAsync()).ToList();

        Assert.NotNull(populatedAddresses[0].coordinates);
    }

}