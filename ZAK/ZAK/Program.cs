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
using ZAK.Auth;

namespace ZAK;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddBlazorBootstrap();
        builder.Services.AddSyncfusionBlazor();

        builder.Services.AddScoped<ICoefficientsDataAccess, CoefficientsDataAccess>();
        builder.Services.AddScoped<IApplicationsDataAccess, ApplicationsDataAccess>();
        builder.Services.AddScoped<IDistrictDataAccess, DistrictDataAccess>();
        builder.Services.AddScoped<IBrigadesDataAccess, BrigadesDataAccess>();


        builder.Services.AddScoped<IFileLoader, FileLoader>();
        builder.Services.AddScoped<IApplicationsScrapper, ApplicationsScrapperUpdated>();
        builder.Services.AddScoped<IGeoDataManager, GeoDataManager>();
        builder.Services.AddScoped<IApplicationsLoader, ApplicationsLoader>();
        builder.Services.AddScoped<IAddressesDataAccess, AddressesDataAccess>();
        builder.Services.AddScoped<IAddressPriorityDataAccess, AddressPriorityDataAccess>();
        builder.Services.AddScoped<IAddressAliasDataAccess, AddressAliasDataAccess>();
        builder.Services.AddScoped<ICoordinatesDataAccess, CoordinatesDataAccess>();
        builder.Services.AddScoped<IBlackoutScheduleDataAccess, BlackoutScheduleDataAccess>();

        builder.Services.AddScoped<IGeoDataManager, GeoDataManager>();

        builder.Services.AddSingleton<IMapRoutesManager, MapRoutesManager.MapRoutesManager>();

        builder.Services.AddSingleton<IAuthorizationHandler, PassHandler>();

        builder.Services.AddDbContext<BlazorAppDbContext>(
        options =>
        {
            options.UseSqlite(@"Data Source=D:\C#_Projects\ZAK\ZAK3\ZAK.Db\DbFiles\testdb2.db");
            //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    );
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("PassPolicy", policy =>
            {
                policy.Requirements.Add(new PassRequirement("123"));
            });
        });
        builder.Services.AddCascadingAuthenticationState();

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

        app.Run();
    }
}
