using System;
using ZAK.DA;
using Microsoft.AspNetCore.Components.Forms;

namespace ZAK.Services.ApplicationsLoadingService;

public interface IApplicationsLoadingService
{
    public Task UpdateApplications(IBrowserFile file);
}
