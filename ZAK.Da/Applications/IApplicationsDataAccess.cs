using ZAK.Db.Models;
using BlazorApp.Enums;

namespace ZAK.DA;

public interface IApplicationsDataAccess
{
    Task<Application> GetApplication(int id);
    List<Application> GetAllApplications(); 
    List<Application> GetAllApplicationsWithIgnoring(); 

    Task ClearApplicationsList();
    
    Task AddApplications(List<Application> applications) ;
    Task AddNewApplications(List<Application> applications);
    Task AddNewApplication(Application application);


    Task ChangeApplicationStrethcingStatus(int id, StretchingStatus status);
    Task SwitchApplicationHotStatus(int id);
    Task SwitchApplicationIgnoredStatus(int id);
    Task SwitchApplicationInScheduleStatus(int id);

    Task ChoseFirstPartOfDay(int id);
    Task ChoseSecondPartOfDay(int id);
    Task SetTimeRange(int id, int from, int to);
    Task SetMaxDays(int id, int daysForConnection);

    Task UpdateApplication(Application application);
}
