using ApplicationsScrappingModule;
using BlazorApp;
using BlazorApp.ApplicationsLoader;
using ZAK.DA;
using BlazorApp.GeoDataManager;
using Microsoft.EntityFrameworkCore;
using ZAK.Components;
using ZAK.MapRoutesManager;
using ZAK.Db;
using Microsoft.AspNetCore.Authentication.Cookies;
using ZAK.Services.UnresolvedAddressesChecker;
using ZAK.Db.Models;
using MudBlazor.Services;
using ZAK.Services.ScheduleManagerService;
using ZAK.DAO;
using ZAK.Services.ApplicationsLoadingService;


namespace ZAK;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddBlazorBootstrap();

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "Login_Pass";
                options.Cookie.MaxAge = TimeSpan.FromDays(30);
            });

        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddMudServices(options =>
    {
        options.PopoverOptions.ThrowOnDuplicateProvider = false;
    });

        string? connectionString = builder.Configuration["ConnectionStrings:MySQL"];
        if (String.IsNullOrEmpty(connectionString)) throw new Exception("Connection string is empty");

        var serverVersion = new MySqlServerVersion(new Version(8, 0, 41));
        builder.Services.AddDbContextFactory<BlazorAppDbContext>(options =>
        options.UseMySql(connectionString, serverVersion).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        builder.Services.AddTransient<IDao<Brigade, BrigadeModel>, DAO.Dao<Brigade, BrigadeModel>>();

        builder.Services.AddTransient<IDao<Coefficient, CoefficientModel>, DAO.Dao<Coefficient, CoefficientModel>>();
        builder.Services.AddTransient<IDao<Application, ApplicationModel>, DAO.Dao<Application, ApplicationModel>>();
        builder.Services.AddTransient<IDao<District, District>, DAO.Dao<District, District>>();
        builder.Services.AddTransient<IDao<Address, AddressModel>, DAO.Dao<Address, AddressModel>>();
        builder.Services.AddTransient<IDao<AddressPriority, AddressPriority>, DAO.Dao<AddressPriority, AddressPriority>>();
        builder.Services.AddTransient<IDao<AddressCoordinates, AddressCoordinatesModel>, DAO.Dao<AddressCoordinates, AddressCoordinatesModel>>();
        builder.Services.AddTransient<IDao<AddressAlias, AddressAliasModel>, DAO.Dao<AddressAlias, AddressAliasModel>>();

        builder.Services.AddTransient<IBlackoutScheduleDataAccess, BlackoutScheduleDataAccess>();

        builder.Services.AddScoped<IFileLoader, FileLoader>();
        builder.Services.AddScoped<IApplicationsScrapper, ApplicationsScrapperUpdated>();
        builder.Services.AddScoped<IGeoDataManager, GeoDataManager>();
        builder.Services.AddScoped<IApplicationsLoader, ApplicationsLoader>();
        
        builder.Services.AddTransient<IApplicationsLoadingService, ApplicationsLoadingService>();
        builder.Services.AddTransient<IScheduleManager, ScheduleManager>();

        builder.Services.AddScoped<IGeoDataManager, GeoDataManager>();
        builder.Services.AddSingleton<IMapRoutesManager, MapRoutesManager.MapRoutesManager>();
        builder.Services.AddScoped<IUnresolvedAddressesChecker, UnresolvedAddressesChecker>();






        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();


        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.UseAuthorization();
        app.UseAuthentication();

        app.Run();
    }
}
