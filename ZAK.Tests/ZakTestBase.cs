using System;
using BlazorApp.DA;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ZAK.DAO;
using ZAK.Db.Models;
using ZAK.Services.ApplicationsManagerSerivce;
using ZAK.Services.ScheduleManagerService;

namespace ZAK.Tests;

public class ZakTestBase : IDisposable
{
    protected TestDbContextFactory dbContextFactory = new();

    protected ILogger<Dao<Address, AddressModel>> addressDaoLogger;
    protected ILogger<Dao<AddressCoordinates, AddressCoordinatesModel>> addressCooerdinatesDaoLogger;
    protected ILogger<Dao<AddressAlias, AddressAliasModel>> addressAliasDaoLogger;
    protected ILogger<Dao<AddressPriority, AddressPriorityModel>> addressPriorityDaoLogger;
    protected ILogger<Dao<Application, ApplicationModel>> applicationDaoLogger;
    protected ILogger<Dao<Brigade, BrigadeModel>> brigadeDaoLogger;
    protected ILogger<Dao<Coefficient, CoefficientModel>> coeficientDaoLogger;
    protected ILogger<Dao<District, DistrictModel>> districtDaoLogger;
    protected ILogger<ScheduleManager> scheduleManagerLogger;
    protected ILogger<ApplicationsManagerService> applicationsManagerLogger;

    public ZakTestBase()
    {
        dbContextFactory.CreateDbContext();

        addressDaoLogger = new NullLogger<Dao<Address, AddressModel>>();
        addressCooerdinatesDaoLogger = new NullLogger<Dao<AddressCoordinates, AddressCoordinatesModel>>();
        addressAliasDaoLogger = new NullLogger<Dao<AddressAlias, AddressAliasModel>>();
        addressPriorityDaoLogger = new NullLogger<Dao<AddressPriority, AddressPriorityModel>>();
        applicationDaoLogger = new NullLogger<Dao<Application, ApplicationModel>>();
        brigadeDaoLogger = new NullLogger<Dao<Brigade, BrigadeModel>>();
        coeficientDaoLogger = new NullLogger<Dao<Coefficient, CoefficientModel>>();
        districtDaoLogger = new NullLogger<Dao<District, DistrictModel>>();
        scheduleManagerLogger = new NullLogger<ScheduleManager>();
        applicationsManagerLogger = new NullLogger<ApplicationsManagerService>();
    }

    public void Dispose()
    {
        dbContextFactory.DeleteTestDb();
    }
}
