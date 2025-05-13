using System.Reflection;
using ZAK.Db;
using ZAK.Db.Models;
using BlazorApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.DA;

public class ApplicationsDataAccess(BlazorAppDbContext blazorAppDbContext) : IApplicationsDataAccess
{
    private readonly BlazorAppDbContext _dbContext = blazorAppDbContext;

    public async Task AddApplications(List<Application> applications)
    {
        
        foreach(Application application in applications)
        {
            AddressModel? address = await _dbContext.addresses.FirstOrDefaultAsync(a => (a.streetName == application.address.streetName) && (a.building == application.address.building) && (a.district.name == application.address.district.name));
        
            if (address is not null)
            {
                application.address = address;
            } 
            else 
            {
                ZAK.Db.Models.DistrictModel? district = await _dbContext.districts.FirstOrDefaultAsync(d => d.name == application.address.district.name);
                if (district is not null)
                {
                    application.address.district = district;
                }
            }    

            await _dbContext.applications.AddAsync(application);
            await _dbContext.SaveChangesAsync();
        }

    }

    public async Task AddNewApplications(List<Application> applications)
    {
        await _dbContext.applications.AddRangeAsync(applications.Where(na => !_dbContext.applications.Any(a => a.id == na.id)));
        await _dbContext.SaveChangesAsync();
    }

    public async Task ChangeApplicationStrethcingStatus(int id, StretchingStatus status)
    {
        ApplicationModel? application = await _dbContext.applications.FindAsync(id);

        if (application is not null) application.stretchingStatus = status;

        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearApplicationsList()
    {
        _dbContext.applications.RemoveRange(_dbContext.applications);
        await _dbContext.SaveChangesAsync();
    }

    public List<Application> GetAllApplications()
    {
        return _dbContext.applications.Select(application => new Application(application)).ToList();
    }

    public List<Application> GetAllApplicationsWithIgnoring()
    {
        return _dbContext.applications.
            Include(a=>a.address).ThenInclude(a => a.coordinates).
            Include(a=>a.address).ThenInclude(a => a.district).
            Where(app => app.ignored != true).Select(application => new Application(application)).ToList();
    }

    public async Task SwitchApplicationHotStatus(int id)
    {
        ApplicationModel? application = await _dbContext.applications.FindAsync(id);

        if (application != null) application.hot = !application.hot;

        await _dbContext.SaveChangesAsync();
    }
    public async Task SwitchApplicationIgnoredStatus(int id)
    {
        ApplicationModel? application = await _dbContext.applications.FindAsync(id);

        if (application != null) application.ignored = !application.ignored;

        await _dbContext.SaveChangesAsync();
    }
    public async Task SwitchApplicationInScheduleStatus(int id)
    {
        ApplicationModel? application = await _dbContext.applications.FindAsync(id);

        if (application != null) application.inSchedule = !application.inSchedule;

        await _dbContext.SaveChangesAsync();
    }
    public async Task ChoseFirstPartOfDay(int id)
    {
        ApplicationModel? application = await _dbContext.applications.FindAsync(id);

        if (application is not null)
        {
            application.firstPart = true;
            application.secondPart = false;
            application.timeRangeIsSet = false;
        }

        await _dbContext.SaveChangesAsync();
    }
    public async Task ChoseSecondPartOfDay(int id)
    {
        ApplicationModel? application = _dbContext.applications.Find(id);

        if (application is not null)
        {
            application.firstPart = false;
            application.secondPart = true;
            application.timeRangeIsSet = false;
        }

        await _dbContext.SaveChangesAsync();
    }
    public async Task SetTimeRange(int id, int from, int to)
    {
        ApplicationModel? application = await _dbContext.applications.FindAsync(id);

        if (application is not null)
        {
            application.startHour = from;
            application.endHour = to;
            application.firstPart = false;
            application.secondPart = false;
            application.timeRangeIsSet = true;
        }

        await _dbContext.SaveChangesAsync();

    }

    public async Task AddNewApplication(Application application)
    {
        await _dbContext.applications.AddAsync(application);
        _dbContext.SaveChanges();
    }

    public async Task<Application> GetApplication(int id)
    {
        ApplicationModel? application =  await _dbContext.applications.
            Include(a=>a.address).
            ThenInclude(a=>a.coordinates).
            FirstOrDefaultAsync(a=>a.id == id);
        if (application is null) application = new ApplicationModel();
        return new Application(application);
    }

    public async Task SetMaxDays(int id, int daysForConnection)
    {
        ApplicationModel? application = await _dbContext.applications.FindAsync(id);

        if (application is not null)
        {
            application.maxDaysForConnection = daysForConnection;
            _dbContext.SaveChanges();
        }

        return;
    }

    public async Task UpdateApplication(Application application)
    {
        if (application is not null)
        {
            ApplicationModel? oldApplication = await _dbContext.applications.FindAsync(application.id);

            if (oldApplication is not null)
            {

                foreach (PropertyInfo property in typeof(ApplicationModel).GetProperties().Where(p => p.CanWrite))
                {
                    property.SetValue(oldApplication, property.GetValue(application));
                }

                _dbContext.SaveChanges();
            }
        }

        return;
    }
}
