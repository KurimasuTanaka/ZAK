using BlazorApp.DA;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Abstractions;
using ZAK.DAO;
using ZAK.Db.Models;

namespace ZAK.Tests;

public class DAOTests
{
    private readonly ITestOutputHelper _output;

    public DAOTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async void InsertNewApplicationToTheEmptyDb()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<Dao<Application, ApplicationModel>> daoLogger1 = new NullLogger<Dao<Application, ApplicationModel>>();
        IDao<Application, ApplicationModel> applicationsDAO = new Dao<Application, ApplicationModel>(dbContextFactory, daoLogger1);

        ILogger<Dao<Address, AddressModel>> daoLogger2 = new NullLogger<Dao<Address, AddressModel>>();
        IDao<Address, AddressModel> addressesDAO = new Dao<Address, AddressModel>(dbContextFactory, daoLogger2);

        //Act

        Address newAddress = new();
        newAddress.streetName = "PaperStreet";
        newAddress.building = "building";

        Application newApplication = new();
        newApplication.address = newAddress;

        await applicationsDAO.Insert(newApplication);

        //Assert
        Assert.Single(await addressesDAO.GetAll());
        Assert.Single(await applicationsDAO.GetAll());

        dbContextFactory.DeleteTestDb();
    }

    [Fact]
    public async void InsertNewAddressAndUpdateItWithoutTracking()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<Dao<Address, AddressModel>> addressLogger = new NullLogger<Dao<Address, AddressModel>>();
        IDao<Address, AddressModel> addressDao = new Dao<Address, AddressModel>(dbContextFactory, addressLogger);

        //Act 
        Address address = new();
        address.streetName = "вулиця Володимира Івасюка";
        address.building = "54";

        await addressDao.Insert(address);

        Address addressToUpdate = (await addressDao.GetAll()).FirstOrDefault()!;
        addressToUpdate.streetName = "проспект Володимира Івасюка";
        await addressDao.Update(addressToUpdate, addressToUpdate.Id);

        Address updatedAddress = (await addressDao.GetAll()).FirstOrDefault()!;

        //Assert
        Assert.NotNull(updatedAddress);
        Assert.Equal("проспект Володимира Івасюка", updatedAddress.streetName);
    }

    [Fact]
    public async void InsertNewAddressAndUpdateRelatedEntityUsingInclueQuery()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<Dao<Address, AddressModel>> addressLogger = new NullLogger<Dao<Address, AddressModel>>();
        IDao<Address, AddressModel> addressDao = new Dao<Address, AddressModel>(dbContextFactory, addressLogger);

        //Act 
        Address address = new();
        address.streetName = "проспект Володимира Івасюка";
        address.building = "54";

        await addressDao.Insert(address);

        Address addressToUpdate = (await addressDao.GetAll()).FirstOrDefault()!;
        addressToUpdate.coordinates = new AddressCoordinates();
        addressToUpdate.coordinates.lat = 5000;
        addressToUpdate.coordinates.lon = 5000;

        await addressDao.Update(
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

        Address updatedAddress = (await addressDao.GetAll(
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
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<Dao<Application, ApplicationModel>> daoLogger1 = new NullLogger<Dao<Application, ApplicationModel>>();
        IDao<Application, ApplicationModel> applicationsDAO = new Dao<Application, ApplicationModel>(dbContextFactory, daoLogger1);

        ILogger<Dao<Address, AddressModel>> daoLogger2 = new NullLogger<Dao<Address, AddressModel>>();
        IDao<Address, AddressModel> addressesDAO = new Dao<Address, AddressModel>(dbContextFactory, daoLogger2);


        Address sharedAddress = new();
        sharedAddress.streetName = "PaperStreet 1";
        sharedAddress.building = "building";

        Application oldApplication = new();
        oldApplication.address = sharedAddress;

        await applicationsDAO.Insert(oldApplication);

        //Act

        Application newApplication = new();
        newApplication.address = sharedAddress;

        Address sharedAddressToEdit = (await addressesDAO.GetAll(query: a => a.Include(a => a.applications))).FirstOrDefault()!;
        //haredAddressToEdit.applications.Add(newApplication);

        //await addressesDAO.Update(sharedAddressToEdit, sharedAddressToEdit.Id);


        await applicationsDAO.Insert(newApplication,
            inputProcessQuery: (query, newApplication, dbContext) =>
            {
                dbContext.Attach(newApplication.address);
                return newApplication;
            }
        );

        //Assert
        Assert.Equal(2, (await applicationsDAO.GetAll()).Count());
        Assert.Single(await addressesDAO.GetAll());

        dbContextFactory.DeleteTestDb();
    }

    [Fact]
    public async void InsertRangeOfApplicationsAndUpdateRangeOfApplicationsInBulk()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<Dao<Application, ApplicationModel>> daoLogger1 = new NullLogger<Dao<Application, ApplicationModel>>();
        IDao<Application, ApplicationModel> applicationsDAO = new Dao<Application, ApplicationModel>(dbContextFactory, daoLogger1);

        Application application1 = new Application();
        application1.operatorComment = "Comment 1 Unedited";

        Application application2 = new Application();
        application2.operatorComment = "Comment 2 Unedited";

        Application application3 = new Application();
        application3.operatorComment = "Comment 3 Unedited";

        //Act

        List<Application> applications = new([application1, application2, application3]);

        await applicationsDAO.InsertRange(applications);

        List<Application> applicationsFromDb = (await applicationsDAO.GetAll()).ToList();

        applicationsFromDb.RemoveAt(1);

        applicationsFromDb.ElementAt(0).operatorComment = "Comment 1 Edited";
        applicationsFromDb.ElementAt(1).operatorComment = "Comment 3 Edited";

        await applicationsDAO.UpdateRange(
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

        applicationsFromDb = (await applicationsDAO.GetAll()).ToList();

        //Assert

        _output.WriteLine(applicationsFromDb.ElementAt(0).operatorComment);
        _output.WriteLine(applicationsFromDb.ElementAt(1).operatorComment);
        _output.WriteLine(applicationsFromDb.ElementAt(2).operatorComment);

        Assert.Equal(3, applicationsFromDb.Count());
        Assert.Equal("Comment 1 Edited", applicationsFromDb.ElementAt(0).operatorComment);
        Assert.Equal("Comment 2 Unedited", applicationsFromDb.ElementAt(1).operatorComment);
        Assert.Equal("Comment 3 Edited", applicationsFromDb.ElementAt(2).operatorComment);

        dbContextFactory.DeleteTestDb();
    }

    [Fact]
    public async void InsertNewAddressesWithInsertRange()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<Dao<Address, AddressModel>> addressLogger = new NullLogger<Dao<Address, AddressModel>>();
        IDao<Address, AddressModel> addressDao = new Dao<Address, AddressModel>(dbContextFactory, addressLogger);

        ILogger<Dao<District, DistrictModel>> districtsLogger = new NullLogger<Dao<District, DistrictModel>>();
        IDao<District, DistrictModel> districtsDao = new Dao<District, DistrictModel>(dbContextFactory, districtsLogger);

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

        await addressDao.InsertRange(addresses);

        //Assert

        List<Address> addressesFromDb = (await addressDao.GetAll()).ToList();
        List<District> districtsFromDb = (await districtsDao.GetAll()).ToList();

        Assert.Equal(3, addressesFromDb.Count);
        Assert.Equal(2, districtsFromDb.Count);
    }

}
