using BlazorApp.DA;
using ZAK.Db.Models;
using BlazorApp.GeoDataManager;
using Xunit;
using Microsoft.Extensions.Logging;
using ZAK.DAO;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ZAK.Tests;

public class GeoDataManagerTests
{
    [Fact]
    public async void CoordinatesLoadingTest()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<Dao<Address, AddressModel>> addressLogger = new NullLogger<Dao<Address, AddressModel>>();
        IDao<Address, AddressModel> addressDao = new Dao<Address, AddressModel>(dbContextFactory, addressLogger);

        ILogger<Dao<Application, ApplicationModel>> applicationsDaoLogger = new NullLogger<Dao<Application, ApplicationModel>>();
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

        await addressDao.Insert(address1);
        await addressDao.Insert(address2);
        await addressDao.Insert(address3);

        GeoDataManager geoDataManager = new(addressDao);

        //Act 

        await geoDataManager.PopulateApplicationsWithGeoData();

        //Assert

        List<Address> populatedAddresses = (await addressDao.GetAll(query: b => b.Include(a => a.coordinates))).ToList();

        Assert.NotNull(populatedAddresses[0].coordinates);
        Assert.NotNull(populatedAddresses[1].coordinates);
        Assert.NotNull(populatedAddresses[2].coordinates);

        dbContextFactory.DeleteTestDb();
    }

    [Fact]
    public async void UpdatingExistedCoordinates()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<Dao<Address, AddressModel>> addressLogger = new NullLogger<Dao<Address, AddressModel>>();
        IDao<Address, AddressModel> addressDao = new Dao<Address, AddressModel>(dbContextFactory, addressLogger);

        ILogger<Dao<Application, ApplicationModel>> applicationsDaoLogger = new NullLogger<Dao<Application, ApplicationModel>>();
        IDao<Application, ApplicationModel> applicationsDAO = new Dao<Application, ApplicationModel>(dbContextFactory, applicationsDaoLogger);

        Address address = new();
        address.streetName = "вулиця Володимира Івасюка";
        address.building = "54";

        await addressDao.Insert(address);

        GeoDataManager geoDataManager = new(addressDao);

        //Act 
        await geoDataManager.PopulateApplicationsWithGeoData();

        Address addressToUpdate = (await addressDao.GetAll()).FirstOrDefault()!;
        addressToUpdate.streetName = "проспект Володимира Івасюка";
        await addressDao.Update(addressToUpdate, addressToUpdate.Id);

        Address addressToUpdate2 = (await addressDao.GetAll()).FirstOrDefault()!;


        await geoDataManager.PopulateApplicationsWithGeoData();

        //Assert

        List<Address> populatedAddresses = (await addressDao.GetAll(query: b => b.Include(a => a.coordinates))).ToList();

        Assert.NotNull(populatedAddresses[0].coordinates);

        dbContextFactory.DeleteTestDb();
    }

}