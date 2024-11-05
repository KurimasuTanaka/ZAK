using BlazorApp.ApplicationsLoader;
using ApplicationsScrappingModule;
using BlazorApp;
using BlazorApp.Components;
using BlazorApp.DA;
using BlazorApp.DB;
using BlazorApp.GeoDataManager;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBlazorBootstrap();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

builder.Services.AddScoped<IGeoDataManager, GeoDataManager>();

builder.Services.AddDbContext<BlazorAppDbContext>(
    options =>
    {
        options.UseSqlite(@"Data Source=D:\C#_Projects\ZAK\ZAK2\BlazorApp.DB\DbFiles\testdb2.db");
        //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles("/Components/Pages");
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
