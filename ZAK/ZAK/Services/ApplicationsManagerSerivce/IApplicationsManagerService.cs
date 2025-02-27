using System;
using BlazorApp.DA;
using Microsoft.AspNetCore.Components.Forms;

namespace ZAK.Services.ApplicationsManagerSerivce;

public interface IApplicationsManagerService
{
    public Task UpdateApplications(IBrowserFile file);
    public Task AddApplication();
    public Task<List<Application>> GetApplications();
}
