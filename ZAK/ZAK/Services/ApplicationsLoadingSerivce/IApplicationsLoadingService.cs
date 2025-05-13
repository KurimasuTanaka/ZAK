using System;
using BlazorApp.DA;
using Microsoft.AspNetCore.Components.Forms;

namespace ZAK.Services.ApplicationsLoadingService;

public interface IApplicationsLoadingService
{
    public Task UpdateApplications(IBrowserFile file);
}
