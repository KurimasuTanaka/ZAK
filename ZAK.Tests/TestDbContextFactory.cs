using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ZAK.Db;

namespace ZAK.Tests;

class TestDbContextFactory : IDbContextFactory<BlazorAppDbContext>
{
    DbContextOptionsBuilder<BlazorAppDbContext> builder = new DbContextOptionsBuilder<BlazorAppDbContext>();
    SqliteConnection _connection = new SqliteConnection("Data Source=testdb.db");
    public TestDbContextFactory()
    {
        builder.UseSqlite(_connection);

        BlazorAppDbContext blazorAppDbContext = new(builder.Options);
        blazorAppDbContext.Database.EnsureDeleted();
        blazorAppDbContext.Database.EnsureCreated();

    }
    public BlazorAppDbContext CreateDbContext()
    {
        BlazorAppDbContext blazorAppDbContext = new(builder.Options);

        return new BlazorAppDbContext(builder.Options);
    }
}
