using System;
using BlazorApp.DA;
using ZAK.Db;
using ZAK.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ZAK.MapRoutesManager;

namespace ZAK.Tests;
        struct ResolvedAdd 
        {
            public AddressModel address;
            public bool isResolved;
        }

public class RoutingTests
{

    // [Fact]
    // public async void CheckAddressesForResolving()
    // {
    //     // Arrange
    //     List<ResolvedAdd> addresses = new();
    //     IMapRoutesManager _mapRoutesManager = new MapRoutesManager.MapRoutesManager();

    //     var options = new DbContextOptionsBuilder<BlazorAppDbContext>().
    //         UseSqlite(@"Data Source=D:\C#_Projects\ZAK\ZAK2\BlazorApp.DB\DbFiles\testdb2.db").
    //         Options;

    //     var builder = new DbContextOptionsBuilder<BlazorAppDbContext>();
    //     builder.UseSqlite(@"Data Source=D:\C#_Projects\ZAK\ZAK2\BlazorApp.DB\DbFiles\testdb2.db");

    //     BlazorAppDbContext context = new BlazorAppDbContext(builder.Options);

    //     //Act

    //     addresses = await context.addresses.Include(a => a.coordinates).Select(a => new ResolvedAdd {
    //         address = a,
    //         isResolved = _mapRoutesManager.CheckResolving((float)a.coordinates.lat, (float)a.coordinates.lon, 1000)
    //     }).ToListAsync();

    //     List<ResolvedAdd> unresolvedAddresses = addresses.Select(a=>a).Where(a => a.isResolved == false).ToList();


    //     //Assert
    //     Assert.Equal(addresses.Select(a=>a).Where(a => a.isResolved == true).ToList().Count, addresses.Count);

    // }
}
