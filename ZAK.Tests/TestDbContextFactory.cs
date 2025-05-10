using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ZAK.Db;

namespace ZAK.Tests;

public class TestDbContextFactory : IDbContextFactory<BlazorAppDbContext>
{
    DbContextOptionsBuilder<BlazorAppDbContext> builder = new DbContextOptionsBuilder<BlazorAppDbContext>();
    public TestDbContextFactory()
    {
        SqliteConnection connection = new ("Data Source=:memory:");
        connection.Open();
        
        builder.UseSqlite(connection);
        builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        BlazorAppDbContext blazorAppDbContext = new(builder.Options);
        blazorAppDbContext.Database.EnsureDeleted();
        blazorAppDbContext.Database.EnsureCreated();
    }

    public void DeleteTestDb()
    {
        BlazorAppDbContext blazorAppDbContext = new(builder.Options);
        blazorAppDbContext.Database.EnsureDeleted();
    }

    public BlazorAppDbContext CreateDbContext()
    {

        return new BlazorAppDbContext(builder.Options);
    }
}
