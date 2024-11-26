using System;
using ZAK.Db.Models;
using static System.Net.Mime.MediaTypeNames;
using BlazorApp.DA;
using Application = BlazorApp.DA.Application;

namespace BlazorApp.Components.Pages.SchedulePage;

public class ApplicationScheduled : Application
{
    public int applicationScheduledTime { get; set; }
    public int brigadeId { get; set; }

    public ApplicationScheduled() : this(new ApplicationModel())
    {
    }

    public ApplicationScheduled(ApplicationModel applicationModel) 
    {
    }


    public ApplicationScheduled(ApplicationModel applicationModel, int brigadeId, int applicationScheduledTime) : base(applicationModel)
    {
        this.brigadeId = brigadeId;
        this.applicationScheduledTime = applicationScheduledTime;
    }
}
