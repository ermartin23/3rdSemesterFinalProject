using System;
using System.Threading.Tasks;
using api;
using api.Features.Boards;
using api.Features.Games;
using api.Features.Players;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;
using Xunit;
using Xunit.DependencyInjection;

namespace tests.ApiTests;

public class Startup : IDisposable
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly string _connectionString;

    public Startup()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        // Start container synchronously before DI is used
        _postgresContainer.StartAsync().GetAwaiter().GetResult();

        _connectionString = _postgresContainer.GetConnectionString();

        // Ensure DB schema exists
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        using var context = new MyDbContext(options);
        context.Database.EnsureCreated();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<MyDbContext>(options =>
        {
            options.UseNpgsql(_connectionString);
        });

        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IGameService, GameService>();

    }

    public void Configure(IHostEnvironment env, ITestOutputHelperAccessor accessor)
    {
        // optional: logging via accessor.Output
    }

    public void Dispose()
    {
        // Stop container when test run is over
        _postgresContainer.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }
}