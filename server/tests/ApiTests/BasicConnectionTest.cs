using Xunit;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Postgres.Scaffolding;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;

namespace tests.ApiTests;

public class BasicConnectionTest : IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;
    private MyDbContext _db = null!;

    public async ValueTask InitializeAsync()
    {
        // Create PostgreSQL container
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        await _container.StartAsync();

        // Build DbContext using NEW connection property
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        _db = new MyDbContext(options);

        await _db.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task CanConnectToDatabase()
    {
        Assert.True(await _db.Database.CanConnectAsync());
    }

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}