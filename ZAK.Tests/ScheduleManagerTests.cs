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
public class ScheduleManagerTests : ZakTestBase
{


    [Fact]
    public async void InsertNewApplicationToEmptySchedule()
    {
        //Arrange

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

        await applicationsDao.Insert(newApplication);

        int timeToScheduleApplication = 3;
        //Act 

        Application applicationToAddToSchedule = (await applicationsDao.GetAll()).First();
        Brigade brigadeToEdit = (await brigadesDao.GetAll()).First();

        await scheduleManager.ScheduleApplication(applicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleApplication);


        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications).ThenInclude(b => b.application)
        )).First();

        Assert.Equal<int>(timeToScheduleApplication, editedBrigade.GetApplicationScheduledOn(3).applicationScheduledTime);
        Assert.Equal<int>(applicationToAddToSchedule.id, editedBrigade.GetApplications().ElementAt(timeToScheduleApplication).id);
    }

    [Fact]
    public async void InsertNewApplicationToScheduleBeforePreviouslyScheduledApplication()
    {
        //Arrange

        //Creating brigade and adding it to the Db
        Brigade emptyBrigade = new();
        await brigadesDao.Insert(emptyBrigade);

        ScheduleManager brigadesManager = new(brigadesDao, scheduleManagerLogger);

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

        await applicationsDao.Insert(newApplication1);
        await applicationsDao.Insert(newApplication2);
        await applicationsDao.Insert(newApplication3);

        int timeToScheduleFirstApplication = 3;
        int timeToScheduleSecondApplication = 4;

        //Act 

        Application firstApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(0);
        Application secondApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(1);
        Application thirdApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(2);

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
    }

}
