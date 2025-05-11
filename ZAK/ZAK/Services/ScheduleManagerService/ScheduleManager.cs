using System;
using BlazorApp.DA;
using Microsoft.EntityFrameworkCore;
using ZAK.DAO;
using ZAK.Db.Models;

namespace ZAK.Services.ScheduleManagerService;

public class ScheduleManager : IScheduleManager
{
    IDao<Brigade, BrigadeModel> _brigadeDataAccess;
    ILogger<ScheduleManager> _logger;
    public ScheduleManager(IDao<Brigade, BrigadeModel> brigadeDataAccess, ILogger<ScheduleManager> logger)
    {
        _brigadeDataAccess = brigadeDataAccess;
        _logger = logger;
    }

    private async Task<Brigade> GetBrigadeById(int brigadeId)
    {
        Brigade? brigade = (await _brigadeDataAccess.GetAll(query => query.Include(b => b.scheduledApplications).ThenInclude(sa =>
        sa.application))).FirstOrDefault(b => b.id == brigadeId);
        if (brigade is null) throw new Exception("New brigade not found");

        return brigade;
    }
    private async Task UpdateBrigade(Brigade brigade)
    {
        _logger.LogInformation($"Updating brigade {brigade.id}...");

        await _brigadeDataAccess.Update(
        brigade,
        brigade.id,
        includeQuery: b => b.Include(b => b.scheduledApplications).ThenInclude(sa => sa.application),
        findPredicate: b => b.id == brigade.id,
        inputDataProccessingQuery: (b, context) =>
        {
            foreach (var schedule in brigade.scheduledApplications)
            {
                if (schedule.application != null)
                {
                    context.Attach(schedule.application);
                }
            }
            return b;
        });

        _logger.LogInformation($"Brigade {brigade.id} updated!");
    }

    private void ShiftScheduledApplicationsForward(Brigade brigade, int time)
    {
        _logger.LogInformation($"Moving applications in brigade {brigade.id} that scheduled after {time} hour to the next hour...");

        brigade.scheduledApplications.Where(sa => sa.scheduledTime > time).ToList().ForEach(sa =>
        {
            sa.scheduledTime++;
        });

        brigade.scheduledApplications.RemoveAll(sa => sa.scheduledTime > 9);

    }

    private void ShiftScheduledApplicationsBackward(Brigade brigade, int time)
    {

        _logger.LogInformation($"Moving applications in brigade {brigade.id} that scheduled after {time} hour to the previous hour...");
        brigade.scheduledApplications.Where(sa => sa.scheduledTime > time).ToList().ForEach(sa =>
        {
            sa.scheduledTime--;
        });
    }

    private void ShiftScheduledApplicationsForward(Brigade brigade, int fromTime, int toTime)
    {
        _logger.LogInformation($"Moving applications in brigade {brigade.id} that scheduled from {fromTime} to {toTime} to the next hour...");

        brigade.scheduledApplications.Where(sa => sa.scheduledTime > fromTime && sa.scheduledTime < toTime).ToList().ForEach(sa =>
        {
            sa.scheduledTime++;
        });
        brigade.scheduledApplications.RemoveAll(sa => sa.scheduledTime > 9);
    }

    private void ShiftScheduledApplicationsBackward(Brigade brigade, int fromTime, int toTime)
    {
        _logger.LogInformation($"Moving applications in brigade {brigade.id} that scheduled from {fromTime} to {toTime} to the previous hour...");

        brigade.scheduledApplications.Where(sa => sa.scheduledTime > fromTime && sa.scheduledTime <= toTime).ToList().ForEach(sa =>
        {
            sa.scheduledTime--;
        });
    }

    public async Task MoveScheduledApplicationFromOneBrigadeToAnother(int applicationId, int brigadeId, int newTime, int prevBrigadeId, int prevTime)
    {
        //Delete application from previous brigade
        Brigade? prevBrigade = await GetBrigadeById(prevBrigadeId);
        prevBrigade.scheduledApplications.RemoveAll(sa => sa.scheduledTime == prevTime);
        ShiftScheduledApplicationsBackward(prevBrigade, prevTime);
        await UpdateBrigade(prevBrigade);

        //Insert application to the new brigade
        Brigade? newBrigade = await GetBrigadeById(brigadeId);
        ShiftScheduledApplicationsForward(newBrigade, newTime);

        _logger.LogInformation($"Inserting application {applicationId} in brigade {brigadeId} on time {newTime}...");
        ScheduledApplicationModel newScheduledApplication = new ScheduledApplicationModel()
        {
            applicationId = applicationId,
            brigadeId = brigadeId,
            scheduledTime = newTime
        };
        newBrigade.scheduledApplications.Add(newScheduledApplication);
        await UpdateBrigade(newBrigade);
    }
    public async Task ScheduleApplication(int applicationId, int brigadeId, int time)
    {
        Brigade newBrigade = await GetBrigadeById(brigadeId);

        _logger.LogInformation($"Inserting application {applicationId} in brigade {brigadeId} on time {time}...");
        //Insert new application to the schedule
        ScheduledApplicationModel newScheduledApplication = new ScheduledApplicationModel()
        {
            applicationId = applicationId,
            brigadeId = brigadeId,
            scheduledTime = time
        };
        newBrigade.scheduledApplications.Add(newScheduledApplication);

        newBrigade.scheduledApplications.RemoveAll(sa => sa.scheduledTime == time && sa.applicationId != applicationId);
        await UpdateBrigade(newBrigade);
    }

    public async Task MakeTimeSlotEmpty(int brigadeId, int time)
    {
        Brigade brigade = await GetBrigadeById(brigadeId);

        ScheduledApplicationModel? scheduledApplication = brigade.scheduledApplications.Where(sa => sa.scheduledTime == time).First();
        if (scheduledApplication is null) return;

        brigade.scheduledApplications.Remove(scheduledApplication);

        await UpdateBrigade(brigade);
    }

    public async Task MoveScheduledApplicationFromOneTimeToAnother(int applicationId, int brigadeId, int newTime, int prevTime)
    {
        Brigade brigade = await GetBrigadeById(brigadeId);


        ShiftScheduledApplicationsBackward(brigade, prevTime, newTime);
        brigade.scheduledApplications.Where(sa => sa.scheduledTime == prevTime).First().scheduledTime = newTime;

        await UpdateBrigade(brigade);
    }

    public async Task MoveEmptyTimeslotFromOneBrigadeToAnother(int brigadeId, int newTime, int prevBrigadeId, int prevTime)
    {
        Brigade prevBrigade = await GetBrigadeById(prevBrigadeId);

        ShiftScheduledApplicationsBackward(prevBrigade, prevTime);

        await UpdateBrigade(prevBrigade);

        Brigade newBrigade = await GetBrigadeById(brigadeId);

        ShiftScheduledApplicationsForward(newBrigade, newTime);

        await UpdateBrigade(newBrigade);
    }

    public async Task MoveEmptyTimeslotFromOneTimeToAnother(int brigadeId, int newTime, int prevTime)
    {
        Brigade brigade = await GetBrigadeById(brigadeId);

        ShiftScheduledApplicationsBackward(brigade, prevTime, newTime);

        await UpdateBrigade(brigade);
    }
}
