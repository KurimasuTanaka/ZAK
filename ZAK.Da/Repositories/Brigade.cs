using System.Reflection;
using ZAK.DAO;
using ZAK.Db;
using ZAK.Db.Models;

namespace BlazorApp.DA;

public class ApplicationScheduled : Application
{
    public int applicationScheduledTime { get; set; }
    public int brigadeId { get; set; }

    public ApplicationScheduled() : this(new ApplicationModel())
    {
    }

    public ApplicationScheduled(ApplicationModel applicationModel, int time = 0) : base(applicationModel)
    {
        applicationScheduledTime = time;
    }
    public ApplicationScheduled(int brigadeId, int time = 0) : base()
    {
        applicationScheduledTime = time;
        this.brigadeId = brigadeId;
    }

    public ApplicationScheduled(ApplicationModel applicationModel, int brigadeId, int applicationScheduledTime) : base(applicationModel)
    {
        this.brigadeId = brigadeId;
        this.applicationScheduledTime = applicationScheduledTime;
    }
}


public class Brigade : BrigadeModel
{
    public Brigade() : base() {
        brigadeNumber = 0;
        brigadeSlotsCount = 9;
    }
    public Brigade(BrigadeModel model) : base(model) {
        brigadeNumber = 0;
        brigadeSlotsCount = 9;
    }

    public object? this[string propertyName]
    {
        get
        {
            PropertyInfo? myPropInfo = GetType().GetProperty(propertyName);
            if (myPropInfo is not null) return myPropInfo.GetValue(this);
            else
            {
                throw new Exception("Non existing property is used");
            }
        }
        set
        {
            PropertyInfo? myPropInfo = GetType().GetProperty(propertyName);
            if (myPropInfo is not null) myPropInfo.SetValue(this, value, null);
            else
            {
                throw new Exception("Non existing property is used");
            }
        }
    }


    public ApplicationScheduled GetApplicationScheduledOn(int time)
    {
        foreach(ScheduledApplicationModel scheduledApplication in scheduledApplications)
        {
            if(scheduledApplication.scheduledTime == time) return new ApplicationScheduled(scheduledApplication.application, time);
        }
        return new ApplicationScheduled();
    }

    public List<ApplicationScheduled> GetApplications()
    {
        List<ApplicationScheduled> applications = new List<ApplicationScheduled>();
        for(int i = 0; i < brigadeSlotsCount; i++)
        {
            ScheduledApplicationModel? scheduledApplication = scheduledApplications.Find(s => s.scheduledTime == i);
            if(scheduledApplication is not null) applications.Add(new ApplicationScheduled(scheduledApplication.application, brigadeId: id, applicationScheduledTime: i));
            else applications.Add(new ApplicationScheduled(this.id, i));
        }
        return applications;
    }

    public async Task PopulateApplicationList(IDao<Application, ApplicationModel> applicationsDataAccess)
    {
        // for(int i = 0; i < 9; i++)
        // {
        //     if(applicationsIds[i] == 0) 
        //     {
        //         applications[i] = null;
        //         continue;
        //     }
        //     else applications[i] = await applicationsDataAccess.GetById(applicationsIds[i]);
        // }
    }
}
