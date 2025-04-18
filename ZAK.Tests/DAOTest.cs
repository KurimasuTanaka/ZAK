using System;
using BlazorApp.DA;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using ZAK.Da.BaseDAO;
using ZAK.Db;
using ZAK.Db.Models;
using ZAK.Services.BrigadesManagerService;

namespace ZAK.Tests;


public class DAOTests
{
    TestDbContextFactory dbContextFactory = new();

    //Applications test

    [Fact]
    public async void InsertNewApplicationToTheEmptyDb()
    {
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
    }

    [Fact]
    public async void InsertNewApplicationWithTheAlreadyExistedAddress()
    {
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
        
        Address sharedAddressToEdit = (await addressesDAO.GetAll( query: a => a.Include(a => a.applications))).FirstOrDefault()!;
        //haredAddressToEdit.applications.Add(newApplication);

        //await addressesDAO.Update(sharedAddressToEdit, sharedAddressToEdit.Id);


        await applicationsDAO.Insert(newApplication, 
            inputProcessQuery: (query, newApplication, dbContext)=> { 
                dbContext.Attach(newApplication.address);
                return newApplication;
            }
        );

        //Assert
        Assert.Equal(2, (await applicationsDAO.GetAll()).Count());
        Assert.Single(await addressesDAO.GetAll());
    }

}
