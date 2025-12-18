using System;
using System.Linq;
using System.Threading.Tasks;
using api.Features.Games;
using api.Features.Games.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace tests.ApiTests;

public class GameServiceTests
{
    // Helper method to create an in-memory DbContext for testing
    private MyDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MyDbContext(options);
    }

    // -------------------------
    // CreateAsync
    // -------------------------

    [Fact]
    public async Task CreateAsync_ShouldCreateGame_WhenValidSundayAndNoActiveGame()
    {
        var db = CreateDbContext();
        var (service, clock) = CreateService(db);

        var dto = new GameCreateRequestDto
        {
            Weekidentity = new DateTime(2025, 1, 5) // Sunday
        };

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Weekidentity, result.Weekidentity);
        Assert.Null(result.Winningnumbers);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenWeekidentityIsNotSunday()
    {
        var db = CreateDbContext();
        var (service, clock) = CreateService(db);

        var dto = new GameCreateRequestDto
        {
            Weekidentity = new DateTime(2025, 1, 6) // Monday
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenActiveGameAlreadyExists()
    {
        var db = CreateDbContext();

        db.Games.Add(new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = DateTime.UtcNow,
            Createdat = DateTime.UtcNow,
            Winningnumbers = null
        });

        await db.SaveChangesAsync();

        var (service, clock) = CreateService(db);

        var dto = new GameCreateRequestDto
        {
            Weekidentity = new DateTime(2025, 1, 5)
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(dto));
    }

    // -------------------------
    // GetAllAsync
    // -------------------------

    [Fact]
    public async Task GetAllAsync_ShouldReturnGamesOrderedByWeekIdentityDesc()
    {
        var db = CreateDbContext();

        db.Games.AddRange(
            new Game { Gameid = Guid.NewGuid(), Weekidentity = new DateTime(2025, 1, 5) },
            new Game { Gameid = Guid.NewGuid(), Weekidentity = new DateTime(2025, 1, 12) }
        );

        await db.SaveChangesAsync();

        var (service, _) = CreateService(db);

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.True(result[0].Weekidentity.CompareTo(result[1].Weekidentity) >= 0);
    }

    // -------------------------
    // GetByIdAsync
    // -------------------------

    [Fact]
    public async Task GetByIdAsync_ShouldReturnGame_WhenExists()
    {
        var db = CreateDbContext();

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = DateTime.UtcNow
        };

        db.Games.Add(game);
        await db.SaveChangesAsync();

        var (service, clock) = CreateService(db);

        var result = await service.GetByIdAsync(game.Gameid);

        Assert.NotNull(result);
        Assert.Equal(game.Gameid, result!.Gameid);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var db = CreateDbContext();
        var (service, _) = CreateService(db);

        var result = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }
    
    [Fact]
    public async Task SetWinningNumbers_ShouldThrow_WhenGameNotFound()
    {
        var db = CreateDbContext();
        var (service, clock) = CreateService(db);

        var dto = new GameSetWinnersDto
        {
            WinningNumbers = new() { 1, 2, 3 }
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.SetWinningNumbersAsync(Guid.NewGuid(), dto));
    }

    [Fact]
    public async Task SetWinningNumbers_ShouldThrow_WhenNumbersDuplicated()
    {
        var db = CreateDbContext();

        var game = new Game { Gameid = Guid.NewGuid() };
        db.Games.Add(game);
        await db.SaveChangesAsync();

        var (service, clock) = CreateService(db);

        var dto = new GameSetWinnersDto
        {
            WinningNumbers = new() { 5, 5, 7 }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SetWinningNumbersAsync(game.Gameid, dto));
    }
    
    private static (GameService svc, FakeClock clock) CreateService(MyDbContext db, DateTime? nowUtc = null)
    {
        var clock = new FakeClock { UtcNow = nowUtc ?? DateTime.UtcNow };
        return (new GameService(db, clock), clock);
    }
}
