namespace BlazorApp.ApplicationsLoader;

using BlazorApp.DA;

public interface IApplicationsLoader
{
    public Task AddNewApplicationsWithRemoval(string applicationsFilePath);
    public Task AddNewApplications(string applicationsFilePath);
    
}
