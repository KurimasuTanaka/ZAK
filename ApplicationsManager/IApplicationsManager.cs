namespace BlazorApp.ApplicationsManager;

using BlazorApp.DA;

public interface IApplicationsManager
{
    public Task AddNewApplicationsWithRemoval(string applicationsFilePath);
    public Task AddNewApplications(string applicationsFilePath);
    public Task AddNewApplication(Application application);

    public Task<IEnumerable<Application>> GetApplications();
    public Task<IEnumerable<Application>> GetApplicationsWithoutIgnored();
    
}
