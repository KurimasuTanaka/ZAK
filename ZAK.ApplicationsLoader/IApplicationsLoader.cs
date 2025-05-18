namespace BlazorApp.ApplicationsLoader;

using ZAK.DA;

public interface IApplicationsLoader
{
    public Task AddNewApplicationsWithRemoval(string applicationsFilePath);
    public Task AddNewApplications(string applicationsFilePath);
    
}
