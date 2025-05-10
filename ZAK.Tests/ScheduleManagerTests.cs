using System;
using BlazorApp.DA;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using ZAK.DAO;
using ZAK.Db.Models;
using ZAK.Services.ScheduleManagerService;

namespace ZAK.Tests;
public class ScheduleManagerTests
{

    [Fact]
    public async void InsertNewApplicationToEmptySchedule()
    {
        TestDbContextFactory dbContextFactory = new();

        //Arrange
        ILogger<ScheduleManager> scheduleManagerLogger = new NullLogger<ScheduleManager>();
        ILogger<Dao<Brigade, BrigadeModel>> brigadeDaoLogger = new NullLogger<Dao<Brigade, BrigadeModel>>();

        IDao<Brigade, BrigadeModel> brigadesDao = new Dao<Brigade, BrigadeModel>(dbContextFactory, brigadeDaoLogger);

        ILogger<Dao<Application, ApplicationModel>> applicationsDaoLogger = new NullLogger<Dao<Application, ApplicationModel>>();
        IDao<Application, ApplicationModel> applicationsDAO = new Dao<Application, ApplicationModel>(dbContextFactory, applicationsDaoLogger);



        //Creating brigade and adding it to the Db
        Brigade emptyBrigade = new();
        await brigadesDao.Insert(emptyBrigade);

        ScheduleManager scheduleManager = new(brigadesDao, scheduleManagerLogger);

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

        await scheduleManager.ScheduleApplication(applicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleApplication);






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
        ILogger<ScheduleManager> brigadeManagerLogger = new NullLogger<ScheduleManager>();
        ILogger<Dao<Brigade, BrigadeModel>> brigadeDaoLogger = new NullLogger<Dao<Brigade, BrigadeModel>>();

        IDao<Brigade, BrigadeModel> brigadesDao = new Dao<Brigade, BrigadeModel>(dbContextFactory, brigadeDaoLogger);

        ILogger<Dao<Application, ApplicationModel>> applicationsDaoLogger = new NullLogger<Dao<Application, ApplicationModel>>();
        IDao<Application, ApplicationModel> applicationsDAO = new Dao<Application, ApplicationModel>(dbContextFactory, applicationsDaoLogger);



        //Creating brigade and adding it to the Db
        Brigade emptyBrigade = new();
        await brigadesDao.Insert(emptyBrigade);

        ScheduleManager brigadesManager = new(brigadesDao, brigadeManagerLogger);

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

        await brigadesManager.ScheduleApplication(firstApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleFirstApplication);
        await brigadesManager.ScheduleApplication(secondApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleSecondApplication);

        await brigadesManager.MoveScheduledApplicationFromOneBrigadeToAnother(thirdApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleFirstApplication, brigadeToEdit.id, 9);

        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications)
        )).First();

        Assert.Equal<int>(timeToScheduleFirstApplication, editedBrigade.GetApplicationScheduledOn(3).applicationScheduledTime);
        Assert.Equal<int>(timeToScheduleSecondApplication, editedBrigade.GetApplicationScheduledOn(4).applicationScheduledTime);
        dbContextFactory.DeleteTestDb();
    }

}
