using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ZAK.Db;

namespace ZAK.Tests;

public class TestDbContextFactory : IDbContextFactory<ZakDbContext>
{
    DbContextOptionsBuilder<ZakDbContext> builder = new DbContextOptionsBuilder<ZakDbContext>();
    public TestDbContextFactory()
    {
        SqliteConnection connection = new ("Data Source=:memory:");
        connection.Open();
        
        builder.UseSqlite(connection);

        ZakDbContext ZakDbContext = new(builder.Options);
        ZakDbContext.Database.EnsureDeleted();
        ZakDbContext.Database.EnsureCreated();
    }

    public void DeleteTestDb()
    {
        ZakDbContext ZakDbContext = new(builder.Options);
        ZakDbContext.Database.EnsureDeleted();
    }

    public ZakDbContext CreateDbContext()
    {

        return new ZakDbContext(builder.Options);
    }
}
