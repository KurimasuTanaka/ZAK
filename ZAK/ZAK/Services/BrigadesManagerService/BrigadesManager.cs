using System;
using BlazorApp.DA;
using Microsoft.EntityFrameworkCore;
using ZAK.Da.BaseDAO;
using ZAK.Db.Models;
using ZstdSharp.Unsafe;

namespace ZAK.Services.BrigadesManagerService;

public class BrigadesManager : IBrigadesManager
{
    IDaoBase<Brigade, BrigadeModel> _brigadeDataAccess;
    ILogger<BrigadesManager> _logger;
    public BrigadesManager(IDaoBase<Brigade, BrigadeModel> brigadeDataAccess, ILogger<BrigadesManager> logger)
    {
        _brigadeDataAccess = brigadeDataAccess;
        _logger = logger;
    }

    public async Task InsertApplication(int applicationId, int brigadeId, int time, int prevBrigadeId, int prevTime)
    {
        _logger.LogInformation($"Inserting application {applicationId} to brigade {brigadeId} at {time} hour");


        _logger.LogInformation($"Deleting application {applicationId} from previous brigade {prevBrigadeId}...");

        //Delete application from previous place
        Brigade? prevBrigade = (await _brigadeDataAccess.GetAll(query => query.Include(b => b.scheduledApplications).ThenInclude(sa =>
        sa.application))).FirstOrDefault(b => b.id == prevBrigadeId);

        if (prevBrigade is null) throw new Exception("Prevoius brigade not found");

        if (applicationId != 0) prevBrigade.scheduledApplications.RemoveAll(sa => sa.brigadeId == prevBrigadeId && sa.scheduledTime == prevTime);


        _logger.LogInformation($"Moving applications in brigade {prevBrigadeId} that scheduled after {prevTime} hour to the previous hour...");

        //Move all applications that scheduled after new one to the previous hour
        for (int i = 0; i < prevBrigade.scheduledApplications.Count; i++)
        {
            if (prevBrigade.scheduledApplications[i].scheduledTime >= prevTime)
            {
                //Delete application from previous scheduled time
                ScheduledApplicationModel scheduledApplication = prevBrigade.scheduledApplications[i];
                scheduledApplication.scheduledTime--;

                prevBrigade.scheduledApplications.RemoveAll(sa => sa.scheduledTime == prevBrigade.scheduledApplications[i].scheduledTime);
                prevBrigade.scheduledApplications.Add(scheduledApplication);
            }
        }

        _logger.LogInformation($"Updating previous brigade {prevBrigadeId}...");


        //Stupid solution
        foreach (ScheduledApplicationModel scheduledApplication in prevBrigade.scheduledApplications)
        {
            scheduledApplication.application = null;
        }


        //Update previous brigade
        await _brigadeDataAccess.Update(
        prevBrigade,
        prevBrigade.id,
        query: b => b.Include(b => b.scheduledApplications),
        findPredicate: b => b.id == prevBrigade.id);

        _logger.LogInformation($"Getting new brigade {brigadeId}...");
        //Get new brigade
        Brigade? newBrigade = (await _brigadeDataAccess.GetAll(query => query.Include(b => b.scheduledApplications).ThenInclude(sa =>
        sa.application))).FirstOrDefault(b => b.id == brigadeId);

        if (newBrigade is null) throw new Exception("New brigade not found");


        //Increse application count in new brigade
        if (brigadeId != prevBrigadeId)
        {
            _logger.LogInformation($"Increasing brigade {brigadeId} slots count...");

            newBrigade.brigadeSlotsCount++;
        }

        _logger.LogInformation($"Moving applications in brigade {brigadeId} that scheduled after {time} hour to the next hour...");
        //Move all applications that scheduled after new one to the next hour

        for (int i = 0; i < newBrigade.scheduledApplications.Count; i++)
        {
            if (newBrigade.scheduledApplications[i].scheduledTime >= time)
            {
                //Delete application from previous scheduled time
                ScheduledApplicationModel scheduledApplication = newBrigade.scheduledApplications[i];
                scheduledApplication.scheduledTime++;

                newBrigade.scheduledApplications.RemoveAll(sa => sa.scheduledTime == newBrigade.scheduledApplications[i].scheduledTime);
                newBrigade.scheduledApplications.Add(scheduledApplication);
            }
        }

        if (applicationId != 0)
        {
            _logger.LogInformation($"Inserting application {applicationId} in brigade {brigadeId} on time {time}...");
            //Insert new application to the schedule
            ScheduledApplicationModel newScheduledApplication = new ScheduledApplicationModel()
            {
                applicationId = applicationId,
                brigadeId = brigadeId,
                scheduledTime = time
            };
            newBrigade.scheduledApplications.Add(newScheduledApplication);
        }
        _logger.LogInformation($"Updating new brigade {brigadeId}...");

        //Stupid solution
        foreach (ScheduledApplicationModel scheduledApplication in newBrigade.scheduledApplications)
        {
            scheduledApplication.application = null;
        }


        //Update new brigade
        await _brigadeDataAccess.Update(
        newBrigade,
        newBrigade.id,
        query: b => b.Include(b => b.scheduledApplications),
        findPredicate: b => b.id == newBrigade.id);
    }
    public async Task InsertNewApplicationInEmptySlot(int applicationId, int brigadeId, int time)
    {
        _logger.LogInformation($"Getting new brigade {brigadeId}...");
        //Get new brigade
        Brigade? newBrigade = (await _brigadeDataAccess.GetAll(query => query.Include(b => b.scheduledApplications).ThenInclude(sa =>
        sa.application))).FirstOrDefault(b => b.id == brigadeId);

        if (newBrigade is null) throw new Exception("New brigade not found");
        _logger.LogInformation($"Inserting application {applicationId} in brigade {brigadeId} on time {time}...");
        //Insert new application to the schedule
        ScheduledApplicationModel newScheduledApplication = new ScheduledApplicationModel()
        {
            applicationId = applicationId,
            brigadeId = brigadeId,
            scheduledTime = time
        };
        newBrigade.scheduledApplications.Add(newScheduledApplication);

        _logger.LogInformation($"Updating new brigade {brigadeId}...");

        foreach (ScheduledApplicationModel scheduledApplication in newBrigade.scheduledApplications)
        {
            scheduledApplication.application = null;
        }

        //Update new brigade
        await _brigadeDataAccess.Update(
        newBrigade,
        newBrigade.id,
        query: b => b.Include(b => b.scheduledApplications),
        findPredicate: b => b.id == newBrigade.id);
    }
}
