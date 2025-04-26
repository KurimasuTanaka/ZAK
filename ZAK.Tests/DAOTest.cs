using System;
using BlazorApp.DA;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Abstractions;
using ZAK.Da.BaseDAO;
using ZAK.Db;
using ZAK.Db.Models;
using ZAK.Services.BrigadesManagerService;

namespace ZAK.Tests;

[Collection("Non-Parallel Collection")]
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
        ILogger<DaoBase<Application, ApplicationModel>> daoLogger1 = new NullLogger<DaoBase<Application, ApplicationModel>>();
        IDaoBase<Application, ApplicationModel> applicationsDAO = new DaoBase<Application, ApplicationModel>(dbContextFactory, daoLogger1);

        ILogger<DaoBase<Address, AddressModel>> daoLogger2 = new NullLogger<DaoBase<Address, AddressModel>>();
        IDaoBase<Address, AddressModel> addressesDAO = new DaoBase<Address, AddressModel>(dbContextFactory, daoLogger2);

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
        ILogger<DaoBase<Address, AddressModel>> addressLogger = new NullLogger<DaoBase<Address, AddressModel>>();
        IDaoBase<Address, AddressModel> addressDao = new DaoBase<Address, AddressModel>(dbContextFactory, addressLogger);

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
        ILogger<DaoBase<Address, AddressModel>> addressLogger = new NullLogger<DaoBase<Address, AddressModel>>();
        IDaoBase<Address, AddressModel> addressDao = new DaoBase<Address, AddressModel>(dbContextFactory, addressLogger);

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
        ILogger<DaoBase<Application, ApplicationModel>> daoLogger1 = new NullLogger<DaoBase<Application, ApplicationModel>>();
        IDaoBase<Application, ApplicationModel> applicationsDAO = new DaoBase<Application, ApplicationModel>(dbContextFactory, daoLogger1);

        ILogger<DaoBase<Address, AddressModel>> daoLogger2 = new NullLogger<DaoBase<Address, AddressModel>>();
        IDaoBase<Address, AddressModel> addressesDAO = new DaoBase<Address, AddressModel>(dbContextFactory, daoLogger2);


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
        ILogger<DaoBase<Application, ApplicationModel>> daoLogger1 = new NullLogger<DaoBase<Application, ApplicationModel>>();
        IDaoBase<Application, ApplicationModel> applicationsDAO = new DaoBase<Application, ApplicationModel>(dbContextFactory, daoLogger1);

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


}
