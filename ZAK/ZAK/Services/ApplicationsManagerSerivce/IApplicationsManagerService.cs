using System;
using BlazorApp.DA;

namespace ZAK.Services.ApplicationsManagerSerivce;

public interface IApplicationsManagerService
{
    public Task UpdateApplications();
    public Task AddApplication();
    public Task<List<Application>> GetApplications();
}
