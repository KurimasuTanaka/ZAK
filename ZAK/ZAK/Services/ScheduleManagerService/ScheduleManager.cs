using System;
using ZAK.DA;
using Microsoft.EntityFrameworkCore;
using ZAK.DAO;
using ZAK.Db.Models;

namespace ZAK.Services.ScheduleManagerService;

public class ScheduleManager : IScheduleManager
{
    IBrigadeRepository _brigadeRepository;
    ILogger<ScheduleManager> _logger;
    public ScheduleManager(IBrigadeRepository brigadeRepository, ILogger<ScheduleManager> logger)
    {
        _brigadeRepository = brigadeRepository;
        _logger = logger;
    }

    private async Task<Brigade> GetBrigadeById(int brigadeId)
    {
        Brigade? brigade = await _brigadeRepository.GetByIdAsync(brigadeId);
        if (brigade is null) throw new Exception("New brigade not found");

        return brigade;
    }

    public async Task ScheduleApplication(int applicationId, int brigadeId, int time, bool onStretching = false)
    {
        Brigade newBrigade = await GetBrigadeById(brigadeId);

        _logger.LogInformation($"Inserting application {applicationId} in brigade {brigadeId} on time {time}...");
        //Insert new application to the schedule
        newBrigade.scheduledApplications.RemoveAll(sa => sa.scheduledTime == time);

        ScheduledApplicationModel newScheduledApplication = new ScheduledApplicationModel()
        {
            applicationId = applicationId,
            brigadeId = brigadeId,
            scheduledTime = time,
            onStretching = onStretching
        };
        newBrigade.scheduledApplications.Add(newScheduledApplication);

        await _brigadeRepository.UpdateAsync(newBrigade);
    }

    public async Task MakeTimeSlotEmpty(int brigadeId, int time)
    {
        Brigade brigade = await GetBrigadeById(brigadeId);

        ScheduledApplicationModel? scheduledApplication = brigade.scheduledApplications.Where(sa => sa.scheduledTime == time).First();
        if (scheduledApplication is null) return;

        brigade.scheduledApplications.Remove(scheduledApplication);

        await _brigadeRepository.UpdateAsync(brigade);
    }

    public async Task ScheduleApplicationToFirstEmptyTime(int applicationId, int brigadeId)
    {
        Brigade brigade = await GetBrigadeById(brigadeId);

        for(int i = 0; i < 10; i++)
        {
            if (brigade.scheduledApplications.FirstOrDefault(sa => sa.scheduledTime == i) is null)
            {
                _logger.LogInformation($"Inserting application {applicationId} in brigade {brigadeId} on time {i}...");
                ScheduledApplicationModel newScheduledApplication = new ScheduledApplicationModel()
                {
                    applicationId = applicationId,
                    brigadeId = brigadeId,
                    scheduledTime = i
                };
                brigade.scheduledApplications.Add(newScheduledApplication);
                await _brigadeRepository.UpdateAsync(brigade);
                return;
            }
        }
    }
}
