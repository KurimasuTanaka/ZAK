using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ZAK.Db;

namespace ZAK.Tests;

class TestDbContextFactory : IDbContextFactory<BlazorAppDbContext>
{
    DbContextOptionsBuilder<BlazorAppDbContext> builder = new DbContextOptionsBuilder<BlazorAppDbContext>();
    public TestDbContextFactory()
    {
        builder.UseSqlite("Data Source=./testdb.db");
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
