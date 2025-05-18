using System;
using ZAK.DA;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ZAK.DAO;
using ZAK.Db.Models;
using ZAK.Services.ApplicationsLoadingService;
using ZAK.Services.ScheduleManagerService;

namespace ZAK.Tests;

public class ZakTestBase : IDisposable
{
    protected TestDbContextFactory dbContextFactory = new();

    protected ILogger<Dao<Address, AddressModel>> addressesDaoLogger;
    protected ILogger<Dao<AddressCoordinates, AddressCoordinatesModel>> addressCooerdinatesDaoLogger;
    protected ILogger<Dao<AddressAlias, AddressAliasModel>> addressAliasesDaoLogger;
    protected ILogger<Dao<AddressPriority, AddressPriorityModel>> addressPrioritiesDaoLogger;
    protected ILogger<Dao<Application, ApplicationModel>> applicationsDaoLogger;
    protected ILogger<Dao<Brigade, BrigadeModel>> brigadesDaoLogger;
    protected ILogger<Dao<Coefficient, CoefficientModel>> coeficientsDaoLogger;
    protected ILogger<Dao<ZAK.DA.District, Db.Models.DistrictModel>> districtsDaoLogger;
    protected ILogger<ScheduleManager> scheduleManagerLogger;
    protected ILogger<ApplicationsLoadingService> applicationsManagerLogger;



    protected IDao<Address, AddressModel> addressesDao;
    protected IDao<AddressCoordinates, AddressCoordinatesModel> addressCoordinatesDao;
    protected IDao<AddressAlias, AddressAliasModel> addressAliasesDao;
    protected IDao<AddressPriority, AddressPriorityModel> addressPrioritiesDao;
    protected IDao<Application, ApplicationModel> applicationsDao;
    protected IDao<Brigade, BrigadeModel> brigadesDao;
    protected IDao<Coefficient, CoefficientModel> coefficientsDao;
    protected IDao<ZAK.DA.District, Db.Models.DistrictModel> districtsDao;


    public ZakTestBase()
    {
        dbContextFactory.CreateDbContext();

        addressesDaoLogger = new NullLogger<Dao<Address, AddressModel>>();
        addressCooerdinatesDaoLogger = new NullLogger<Dao<AddressCoordinates, AddressCoordinatesModel>>();
        addressAliasesDaoLogger = new NullLogger<Dao<AddressAlias, AddressAliasModel>>();
        addressPrioritiesDaoLogger = new NullLogger<Dao<AddressPriority, AddressPriorityModel>>();
        applicationsDaoLogger = new NullLogger<Dao<Application, ApplicationModel>>();
        brigadesDaoLogger = new NullLogger<Dao<Brigade, BrigadeModel>>();
        coeficientsDaoLogger = new NullLogger<Dao<Coefficient, CoefficientModel>>();
        districtsDaoLogger = new NullLogger<Dao<ZAK.DA.District, Db.Models.DistrictModel>>();
        scheduleManagerLogger = new NullLogger<ScheduleManager>();
        applicationsManagerLogger = new NullLogger<ApplicationsLoadingService>();

        addressesDao = new Dao<Address, AddressModel>(dbContextFactory, addressesDaoLogger);
        addressCoordinatesDao = new Dao<AddressCoordinates, AddressCoordinatesModel>(dbContextFactory, addressCooerdinatesDaoLogger);
        addressAliasesDao = new Dao<AddressAlias, AddressAliasModel>(dbContextFactory, addressAliasesDaoLogger);
        addressPrioritiesDao = new Dao<AddressPriority, AddressPriorityModel>(dbContextFactory, addressPrioritiesDaoLogger);
        applicationsDao = new Dao<Application, ApplicationModel>(dbContextFactory, applicationsDaoLogger);
        brigadesDao = new Dao<Brigade, BrigadeModel>(dbContextFactory, brigadesDaoLogger);
        coefficientsDao = new Dao<Coefficient, CoefficientModel>(dbContextFactory, coeficientsDaoLogger);
        districtsDao = new Dao<ZAK.DA.District, Db.Models.DistrictModel>(dbContextFactory, districtsDaoLogger);
    }

    public void Dispose()
    {
        dbContextFactory.DeleteTestDb();
    }
}
