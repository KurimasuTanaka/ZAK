using ApplicationsScrappingModule;
using BlazorApp;
using BlazorApp.ApplicationsLoader;
using BlazorApp.DA;
using BlazorApp.GeoDataManager;
using Microsoft.EntityFrameworkCore;
using ZAK.Components;
using Syncfusion.Blazor;
using ZAK.MapRoutesManager;
using ZAK.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using ZAK.Services.UnresolvedAddressesChecker;

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

        builder.Services.AddTransient<ICoefficientsDataAccess,      CoefficientsDataAccess>();
        builder.Services.AddTransient<IApplicationsDataAccess,      ApplicationsDataAccess>();
        builder.Services.AddTransient<IDistrictDataAccess,          DistrictDataAccess>();
        builder.Services.AddTransient<IBrigadesDataAccess,          BrigadesDataAccess>();
        builder.Services.AddTransient<IAddressesDataAccess,         AddressesDataAccess>();
        builder.Services.AddTransient<IAddressPriorityDataAccess,   AddressPriorityDataAccess>();
        builder.Services.AddTransient<ICoordinatesDataAccess,       CoordinatesDataAccess>();
        builder.Services.AddTransient<IAddressAliasDataAccess,      AddressAliasDataAccess>();
        builder.Services.AddTransient<IBlackoutScheduleDataAccess,  BlackoutScheduleDataAccess>();


        builder.Services.AddScoped<IFileLoader, FileLoader>();
        builder.Services.AddScoped<IApplicationsScrapper, ApplicationsScrapperUpdated>();
        builder.Services.AddScoped<IGeoDataManager, GeoDataManager>();
        builder.Services.AddScoped<IApplicationsLoader, ApplicationsLoader>();

        builder.Services.AddScoped<IGeoDataManager, GeoDataManager>();
        builder.Services.AddSingleton<IMapRoutesManager, MapRoutesManager.MapRoutesManager>();
        builder.Services.AddScoped<IUnresolvedAddressesChecker, UnresolvedAddressesChecker>();


        string? connectionString = builder.Configuration["ConnectionStrings:MySQL"];
        if(String.IsNullOrEmpty(connectionString)) throw new Exception("Connection string is empty");

        var serverVersion = new MySqlServerVersion(new Version(8, 0, 41));
        builder.Services.AddDbContext<BlazorAppDbContext>(options =>
        options.UseMySql(connectionString, serverVersion), ServiceLifetime.Transient);



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
