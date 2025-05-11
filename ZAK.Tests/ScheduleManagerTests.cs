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
        ScheduleManager scheduleManager = new(brigadesDao, scheduleManagerLogger);

        Brigade emptyBrigade = new();
        await brigadesDao.Insert(emptyBrigade);


        //Adding new application to the Db

        Application newApplication = new();

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
        ScheduleManager brigadesManager = new(brigadesDao, scheduleManagerLogger);

        Brigade emptyBrigade = new();
        await brigadesDao.Insert(emptyBrigade);

        Application newApplication1 = new();
        newApplication1.operatorComment = "First application";

        Application newApplication2 = new();
        newApplication2.operatorComment = "Second application";

        await applicationsDao.Insert(newApplication1);
        await applicationsDao.Insert(newApplication2);

        int timeToScheduleFirstApplication = 3;
        int timeToScheduleSecondApplication = 2;

        //Act 

        Application firstApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(0);
        Application secondApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(1);

        Brigade brigadeToEdit = (await brigadesDao.GetAll()).First();

        await brigadesManager.ScheduleApplication(firstApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleFirstApplication);

        await brigadesManager.ScheduleApplication(
            secondApplicationToAddToSchedule.id,
            brigadeToEdit.id,
            timeToScheduleSecondApplication);

        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application)
        )).First();

        editedBrigade.scheduledApplications.OrderBy(sa => sa.scheduledTime);

        Assert.Equal(secondApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[0].application.id);
        Assert.Equal(firstApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[1].application.id);

        Assert.Equal(timeToScheduleSecondApplication, editedBrigade.scheduledApplications[0].scheduledTime);
        Assert.Equal(timeToScheduleFirstApplication, editedBrigade.scheduledApplications[1].scheduledTime);
    }
    [Fact]
    public async void InsertNewApplicationToScheduleAfterPreviouslyScheduledApplication()
    {
        //Arrange
        ScheduleManager brigadesManager = new(brigadesDao, scheduleManagerLogger);

        Brigade emptyBrigade = new();
        await brigadesDao.Insert(emptyBrigade);

        Application newApplication1 = new();
        newApplication1.operatorComment = "First application";

        Application newApplication2 = new();
        newApplication2.operatorComment = "Second application";

        await applicationsDao.Insert(newApplication1);
        await applicationsDao.Insert(newApplication2);

        int timeToScheduleFirstApplication = 3;
        int timeToScheduleSecondApplication = 4;

        //Act 

        Application firstApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(0);
        Application secondApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(1);

        Brigade brigadeToEdit = (await brigadesDao.GetAll()).First();

        await brigadesManager.ScheduleApplication(firstApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleFirstApplication);

        await brigadesManager.ScheduleApplication(
            secondApplicationToAddToSchedule.id,
            brigadeToEdit.id,
            timeToScheduleSecondApplication);

        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application)
        )).First();

        editedBrigade.scheduledApplications.OrderBy(sa => sa.scheduledTime);

        Assert.Equal(firstApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[0].application.id);
        Assert.Equal(secondApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[1].application.id);

        Assert.Equal(timeToScheduleFirstApplication, editedBrigade.scheduledApplications[0].scheduledTime);
        Assert.Equal(timeToScheduleSecondApplication, editedBrigade.scheduledApplications[1].scheduledTime);

    }

    [Fact]
    public async void ScheduleApplicationOnTimeWhereApplicationAlreadyExist()
    {
        //Arrange
        ScheduleManager brigadesManager = new(brigadesDao, scheduleManagerLogger);

        Brigade emptyBrigade = new();
        await brigadesDao.Insert(emptyBrigade);

        Application newApplication1 = new();
        newApplication1.operatorComment = "First application";

        Application newApplication2 = new();
        newApplication2.operatorComment = "Second application";

        await applicationsDao.Insert(newApplication1);
        await applicationsDao.Insert(newApplication2);

        int timeToScheduleApplication = 3;

        //Act 

        Application firstApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(0);
        Application secondApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(1);

        Brigade brigadeToEdit = (await brigadesDao.GetAll()).First();

        await brigadesManager.ScheduleApplication(firstApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleApplication);

        await brigadesManager.ScheduleApplication(
            secondApplicationToAddToSchedule.id,
            brigadeToEdit.id,
            timeToScheduleApplication);

        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application)
        )).First();

        editedBrigade.scheduledApplications.OrderBy(sa => sa.scheduledTime);

        Assert.Single(editedBrigade.scheduledApplications);
        Assert.Equal(secondApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[0].applicationId);

    }

    [Fact]
    public async void MoveScheduledApplicationFromOneBrigadeToAnother()
    {
        //Arrange
        ScheduleManager brigadesManager = new(brigadesDao, scheduleManagerLogger);

        Brigade brigade1 = new();
        Brigade brigade2 = new();

        await brigadesDao.Insert(brigade1);
        await brigadesDao.Insert(brigade2);

        Application newApplication1 = new();
        newApplication1.operatorComment = "First application";

        Application newApplication2 = new();
        newApplication2.operatorComment = "Second application";

        Application newApplication3 = new();
        newApplication3.operatorComment = "Third application";

        Application newApplication4 = new();
        newApplication3.operatorComment = "Forth application";


        await applicationsDao.Insert(newApplication1);
        await applicationsDao.Insert(newApplication2);
        await applicationsDao.Insert(newApplication3);
        await applicationsDao.Insert(newApplication4);

        int timeToScheduleFirstApplication = 1;
        int timeToScheduleSecondApplication = 9;
        int timeToScheduleThirdApplication = 5;
        int timeToScheduleForthApplication = 7;


        Application firstApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(0);
        Application secondApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(1);
        Application thirdApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(2);
        Application forthApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(2);

        Brigade brigadeToEdit1 = (await brigadesDao.GetAll()).ElementAt(0);
        Brigade brigadeToEdit2 = (await brigadesDao.GetAll()).ElementAt(1);

        await brigadesManager.ScheduleApplication(firstApplicationToAddToSchedule.id, brigadeToEdit2.id, timeToScheduleFirstApplication);
        await brigadesManager.ScheduleApplication(secondApplicationToAddToSchedule.id, brigadeToEdit2.id, timeToScheduleSecondApplication);
        await brigadesManager.ScheduleApplication(forthApplicationToAddToSchedule.id, brigadeToEdit2.id, timeToScheduleForthApplication);
        await brigadesManager.ScheduleApplication(thirdApplicationToAddToSchedule.id, brigadeToEdit1.id, timeToScheduleThirdApplication);

        //Act 

        await brigadesManager.MoveScheduledApplicationFromOneBrigadeToAnother(
            thirdApplicationToAddToSchedule.id,
            brigadeToEdit2.id,
            timeToScheduleThirdApplication,
            brigadeToEdit1.id,
            timeToScheduleThirdApplication);

        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application)
        )).ElementAt(1);

        editedBrigade.scheduledApplications.OrderBy(sa => sa.scheduledTime);

        Assert.Equal(3, editedBrigade.scheduledApplications.Count);
        Assert.Equal(firstApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[0].applicationId);
        Assert.Equal(thirdApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[1].applicationId);

        Assert.Equal(forthApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[2].applicationId);
        Assert.Equal(timeToScheduleForthApplication + 1, editedBrigade.scheduledApplications[2].scheduledTime);

    }

    [Fact]
    public async void DeleteScheduledApplicationFromSchedule()
    {
        //Arrange
        ScheduleManager brigadesManager = new(brigadesDao, scheduleManagerLogger);

        Brigade emptyBrigade = new();
        await brigadesDao.Insert(emptyBrigade);

        Application newApplication = new();
        newApplication.operatorComment = "First application";

        await applicationsDao.Insert(newApplication);

        int timeToScheduleApplication = 3;

        //Act 

        Application applicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(0);

        Brigade brigadeToEdit = (await brigadesDao.GetAll()).First();

        await brigadesManager.ScheduleApplication(applicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleApplication);

        await brigadesManager.MakeTimeSlotEmpty(
            brigadeToEdit.id,
            timeToScheduleApplication);

        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application)
        )).First();

        editedBrigade.scheduledApplications.OrderBy(sa => sa.scheduledTime);

        Assert.Empty(editedBrigade.scheduledApplications);
    }

    [Fact]
    public async void MoveScheduledApplicationFromOneTimeToAnother()
    {
        //Arrange
        ScheduleManager brigadesManager = new(brigadesDao, scheduleManagerLogger);

        Brigade brigade = new();

        await brigadesDao.Insert(brigade);

        Application newApplication1 = new();
        newApplication1.operatorComment = "First application";

        Application newApplication2 = new();
        newApplication2.operatorComment = "Second application";

        Application newApplication3 = new();
        newApplication3.operatorComment = "Third application";

        Application newApplication4 = new();
        newApplication4.operatorComment = "Forth application";


        await applicationsDao.Insert(newApplication1);
        await applicationsDao.Insert(newApplication2);
        await applicationsDao.Insert(newApplication3);
        await applicationsDao.Insert(newApplication4);

        int timeToScheduleFirstApplication = 1;
        int timeToScheduleSecondApplication = 2;
        int timeToScheduleThirdApplication = 5;
        int timeToScheduleForthApplication = 9;

        int timeToMoveApplication = 7;

        Application firstApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(0);
        Application secondApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(1);
        Application thirdApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(2);
        Application forthApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(3);

        Brigade brigadeToEdit = (await brigadesDao.GetAll()).ElementAt(0);

        await brigadesManager.ScheduleApplication(firstApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleFirstApplication);
        await brigadesManager.ScheduleApplication(secondApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleSecondApplication);
        await brigadesManager.ScheduleApplication(thirdApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleThirdApplication);
        await brigadesManager.ScheduleApplication(forthApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleForthApplication);

        //Act 

        await brigadesManager.MoveScheduledApplicationFromOneTimeToAnother(
            secondApplicationToAddToSchedule.id,
            brigadeToEdit.id,
            timeToMoveApplication, timeToScheduleSecondApplication);

        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application)
        )).ElementAt(0);

        editedBrigade.scheduledApplications.OrderBy(sa => sa.scheduledTime);

        Assert.Equal(4, editedBrigade.scheduledApplications.Count);

        Assert.Equal(firstApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[0].applicationId);
        Assert.Equal(thirdApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[1].applicationId);
        Assert.Equal(secondApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[2].applicationId);
        Assert.Equal(forthApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[3].applicationId);

        Assert.Equal(timeToScheduleFirstApplication, editedBrigade.scheduledApplications[0].scheduledTime);
        Assert.Equal(timeToScheduleThirdApplication - 1, editedBrigade.scheduledApplications[1].scheduledTime);
        Assert.Equal(timeToMoveApplication, editedBrigade.scheduledApplications[2].scheduledTime);
        Assert.Equal(timeToScheduleForthApplication, editedBrigade.scheduledApplications[3].scheduledTime);
    }

    [Fact]
    public async void MoveEmptyTimeslotFromOneTimeToAnother()
    {
        //Arrange
        ScheduleManager brigadesManager = new(brigadesDao, scheduleManagerLogger);

        Brigade brigade = new();

        await brigadesDao.Insert(brigade);

        Application newApplication1 = new();
        newApplication1.operatorComment = "First application";


        Application newApplication2 = new();
        newApplication2.operatorComment = "Second application";

        Application newApplication3 = new();
        newApplication2.operatorComment = "Third application";


        await applicationsDao.Insert(newApplication1);
        await applicationsDao.Insert(newApplication2);
        await applicationsDao.Insert(newApplication3);

        int timeToScheduleFirstApplication = 1;
        int timeToScheduleEmptySlot = 2;
        int timeToScheduleThirdApplication = 5;
        int timeToScheduleForthApplication = 9;

        int timeToMoveEmptySlot = 7;

        Application firstApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(0);
        Application thirdApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(1);
        Application forthApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(2);

        Brigade brigadeToEdit = (await brigadesDao.GetAll()).ElementAt(0);

        await brigadesManager.ScheduleApplication(firstApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleFirstApplication);
        await brigadesManager.ScheduleApplication(thirdApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleThirdApplication);
        await brigadesManager.ScheduleApplication(forthApplicationToAddToSchedule.id, brigadeToEdit.id, timeToScheduleForthApplication);

        //Act 

        await brigadesManager.MoveEmptyTimeslotFromOneTimeToAnother(
            brigadeToEdit.id,
            timeToMoveEmptySlot, timeToScheduleEmptySlot);

        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application)
        )).ElementAt(0);

        editedBrigade.scheduledApplications.OrderBy(sa => sa.scheduledTime);

        Assert.Equal(3, editedBrigade.scheduledApplications.Count);

        Assert.Equal(firstApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[0].applicationId);
        Assert.Equal(thirdApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[1].applicationId);
        Assert.Equal(forthApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[2].applicationId);

        Assert.Equal(timeToScheduleFirstApplication, editedBrigade.scheduledApplications[0].scheduledTime);
        Assert.Equal(timeToScheduleThirdApplication - 1, editedBrigade.scheduledApplications[1].scheduledTime);
        Assert.Equal(timeToScheduleForthApplication, editedBrigade.scheduledApplications[2].scheduledTime);
    }

    [Fact]
    public async void MoveEmptyTimeSlotFromOneBrigadeToAnother()
    {
        //Arrange
        ScheduleManager brigadesManager = new(brigadesDao, scheduleManagerLogger);

        Brigade brigade1 = new();
        Brigade brigade2 = new();

        await brigadesDao.Insert(brigade1);
        await brigadesDao.Insert(brigade2);

        Application newApplication1 = new();
        newApplication1.operatorComment = "First application";

        Application newApplication2 = new();
        newApplication2.operatorComment = "Second application";

        Application newApplication3 = new();
        newApplication3.operatorComment = "Third application";


        await applicationsDao.Insert(newApplication1);
        await applicationsDao.Insert(newApplication2);
        await applicationsDao.Insert(newApplication3);

        int timeToScheduleFirstApplication = 1;
        int timeToScheduleSecondApplication = 9;
        int timeToScheduleEmptySlot = 5;
        int timeToScheduleThirdApplication = 7;


        Application firstApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(0);
        Application secondApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(1);
        Application forthApplicationToAddToSchedule = (await applicationsDao.GetAll()).ElementAt(2);

        Brigade brigadeToEdit1 = (await brigadesDao.GetAll()).ElementAt(0);
        Brigade brigadeToEdit2 = (await brigadesDao.GetAll()).ElementAt(1);

        await brigadesManager.ScheduleApplication(firstApplicationToAddToSchedule.id, brigadeToEdit2.id, timeToScheduleFirstApplication);
        await brigadesManager.ScheduleApplication(secondApplicationToAddToSchedule.id, brigadeToEdit2.id, timeToScheduleSecondApplication);
        await brigadesManager.ScheduleApplication(forthApplicationToAddToSchedule.id, brigadeToEdit2.id, timeToScheduleThirdApplication);

        //Act 

        await brigadesManager.MoveEmptyTimeslotFromOneBrigadeToAnother(
            brigadeToEdit2.id,
            timeToScheduleEmptySlot,
            brigadeToEdit1.id,
            timeToScheduleEmptySlot);

        //Assert

        Brigade editedBrigade = (await brigadesDao.GetAll(
            query: b => b.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application)
        )).ElementAt(1);

        editedBrigade.scheduledApplications.OrderBy(sa => sa.scheduledTime);

        Assert.Equal(2, editedBrigade.scheduledApplications.Count);
        Assert.Equal(firstApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[0].applicationId);

        Assert.Equal(forthApplicationToAddToSchedule.id, editedBrigade.scheduledApplications[1].applicationId);
        Assert.Equal(timeToScheduleThirdApplication + 1, editedBrigade.scheduledApplications[1].scheduledTime);

    }

}
