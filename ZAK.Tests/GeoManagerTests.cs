using BlazorApp.DA;
using ZAK.Db.Models;
using BlazorApp.GeoDataManager;
using Xunit;
using Microsoft.Extensions.Logging;
using ZAK.Da.BaseDAO;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ZAK.Tests;
[Collection("Non-Parallel Collection")]
public class GeoDataManagerTests
{
    [Fact]
    public async void CoordinatesLoadingTest()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<DaoBase<Address, AddressModel>> addressLogger = new NullLogger<DaoBase<Address, AddressModel>>();
        IDaoBase<Address, AddressModel> addressDao = new DaoBase<Address, AddressModel>(dbContextFactory, addressLogger);

        ILogger<DaoBase<Application, ApplicationModel>> applicationsDaoLogger = new NullLogger<DaoBase<Application, ApplicationModel>>();
        IDaoBase<Application, ApplicationModel> applicationsDAO = new DaoBase<Application, ApplicationModel>(dbContextFactory, applicationsDaoLogger);

        Address address1 = new();
        address1.streetName = "вулиця Володимира Івасюка";
        address1.building = "54";

        Address address2 = new();
        address2.streetName = "вулиця Володимира Івасюка";
        address2.building = "54";

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
        ILogger<DaoBase<Address, AddressModel>> addressLogger = new NullLogger<DaoBase<Address, AddressModel>>();
        IDaoBase<Address, AddressModel> addressDao = new DaoBase<Address, AddressModel>(dbContextFactory, addressLogger);

        ILogger<DaoBase<Application, ApplicationModel>> applicationsDaoLogger = new NullLogger<DaoBase<Application, ApplicationModel>>();
        IDaoBase<Application, ApplicationModel> applicationsDAO = new DaoBase<Application, ApplicationModel>(dbContextFactory, applicationsDaoLogger);

        Address address1 = new();
        address1.streetName = "проспект Володимира Івасюка";
        address1.building = "54";

        await addressDao.Insert(address1);

        GeoDataManager geoDataManager = new(addressDao);

        await geoDataManager.PopulateApplicationsWithGeoData();

        //Act 

        await geoDataManager.PopulateApplicationsWithGeoData();

        //Assert

        List<Address> populatedAddresses = (await addressDao.GetAll(query: b => b.Include(a => a.coordinates))).ToList();

        Assert.NotNull(populatedAddresses[0].coordinates);

        dbContextFactory.DeleteTestDb();
    }

}