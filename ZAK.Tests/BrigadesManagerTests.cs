using System;
using BlazorApp.DA;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using ZAK.Da.BaseDAO;
using ZAK.Db.Models;
using ZAK.Services.BrigadesManagerService;

namespace ZAK.Tests;
[Collection("Non-Parallel Collection")]
public class BrigadesManagerTests
{

    [Fact]
    public async void InsertNewApplicationToEmptySchedule()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<BrigadesManager> brigadeManagerLogger = new NullLogger<BrigadesManager>();
        ILogger<DaoBase<Brigade, BrigadeModel>> brigadeDaoLogger = new NullLogger<DaoBase<Brigade, BrigadeModel>>();

        IDaoBase<Brigade, BrigadeModel> brigadesDao = new DaoBase<Brigade, BrigadeModel>(dbContextFactory, brigadeDaoLogger);

        ILogger<DaoBase<Application, ApplicationModel>> applicationsDaoLogger = new NullLogger<DaoBase<Application, ApplicationModel>>();
        IDaoBase<Application, ApplicationModel> applicationsDAO = new DaoBase<Application, ApplicationModel>(dbContextFactory, applicationsDaoLogger);



        //Creating brigade and adding it to the Db
        Brigade emptyBrigade = new();
        await brigadesDao.Insert(emptyBrigade);

        BrigadesManager brigadesManager = new(brigadesDao, brigadeManagerLogger);

        //Adding new application to the Db
        Address newAddress = new();
        newAddress.streetName = "PaperStreet";
        newAddress.building = "building";

        Application newApplication = new();
        newApplication.address = newAddress;

        await applicationsDAO.Insert(newApplication);

        int timeToScheduleApplication = 3;




        //Act 

        Application applicationToAddToSchedule = (await applicationsDAO.GetAll()).First();
        Brigade brigadeToEdit = (await brigadesDao.GetAll()).First();

        await brigadesManager.InsertNewApplicationInEmptySlot(applicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleApplication);






        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications).ThenInclude(b => b.application)
        )).First();

        Assert.Equal<int>(timeToScheduleApplication, editedBrigade.GetApplicationScheduledOn(3).applicationScheduledTime);
        Assert.Equal<int>(applicationToAddToSchedule.id, editedBrigade.GetApplications().ElementAt(timeToScheduleApplication).id);

        dbContextFactory.DeleteTestDb();
    }

    [Fact]
    public async void InsertNewApplicationToScheduleBeforePreviouslyScheduledApplication()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<BrigadesManager> brigadeManagerLogger = new NullLogger<BrigadesManager>();
        ILogger<DaoBase<Brigade, BrigadeModel>> brigadeDaoLogger = new NullLogger<DaoBase<Brigade, BrigadeModel>>();

        IDaoBase<Brigade, BrigadeModel> brigadesDao = new DaoBase<Brigade, BrigadeModel>(dbContextFactory, brigadeDaoLogger);

        ILogger<DaoBase<Application, ApplicationModel>> applicationsDaoLogger = new NullLogger<DaoBase<Application, ApplicationModel>>();
        IDaoBase<Application, ApplicationModel> applicationsDAO = new DaoBase<Application, ApplicationModel>(dbContextFactory, applicationsDaoLogger);



        //Creating brigade and adding it to the Db
        Brigade emptyBrigade = new();
        await brigadesDao.Insert(emptyBrigade);

        BrigadesManager brigadesManager = new(brigadesDao, brigadeManagerLogger);

        //Adding new application to the Db
        Address newAddress = new();
        newAddress.streetName = "PaperStreet 1";
        newAddress.building = "building";

        Application newApplication1 = new();
        newApplication1.address = newAddress;

        Application newApplication2 = new();
        newApplication1.address = newAddress;

        Application newApplication3 = new();
        newApplication1.address = newAddress;


        await applicationsDAO.Insert(newApplication1);
        await applicationsDAO.Insert(newApplication2);
        await applicationsDAO.Insert(newApplication3);

        int timeToScheduleFirstApplication = 3;
        int timeToScheduleSecondApplication = 4;




        //Act 

        Application firstApplicationToAddToSchedule = (await applicationsDAO.GetAll()).ElementAt(0);
        Application secondApplicationToAddToSchedule = (await applicationsDAO.GetAll()).ElementAt(1);
        Application thirdApplicationToAddToSchedule = (await applicationsDAO.GetAll()).ElementAt(2);

        Brigade brigadeToEdit = (await brigadesDao.GetAll()).First();

        await brigadesManager.InsertNewApplicationInEmptySlot(firstApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleFirstApplication);
        await brigadesManager.InsertNewApplicationInEmptySlot(secondApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleSecondApplication);

        await brigadesManager.InsertApplication(thirdApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleFirstApplication, brigadeToEdit.id, 9);

        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications)
        )).First();

        Assert.Equal<int>(timeToScheduleFirstApplication, editedBrigade.GetApplicationScheduledOn(3).applicationScheduledTime);
        Assert.Equal<int>(timeToScheduleSecondApplication, editedBrigade.GetApplicationScheduledOn(4).applicationScheduledTime);
        dbContextFactory.DeleteTestDb();
    }

}
