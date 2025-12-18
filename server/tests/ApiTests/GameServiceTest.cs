using System;
using System.Linq;
using System.Threading.Tasks;
using api.Features.Games;
using api.Features.Games.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace tests.ApiTests.Games;

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
    // HAPPY PATH: CreateAsync
    // -------------------------
    [Fact]
    public async Task CreateAsync_ShouldCreateGame_WhenValidSundayAndNoActiveGame()
    {
        // Arrange
        var db = CreateDbContext();
        var (service, clock) = CreateService(db);

        var dto = new GameCreateRequestDto
        {
            Weekidentity = new DateTime(2025, 1, 5) // Sunday
        };

        // Act
        var result = await service.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Weekidentity, result.Weekidentity);
        Assert.Null(result.Winningnumbers);
    }

    // --------------------------------------
    // UNHAPPY PATH: CreateAsync - Not Sunday
    // --------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenWeekidentityIsNotSunday()
    {
        // Arrange
        var db = CreateDbContext();
        var (service, clock) = CreateService(db);

        var dto = new GameCreateRequestDto
        {
            Weekidentity = new DateTime(2025, 1, 6) // Monday
        };

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
    }

    // ---------------------------------------------------
    // UNHAPPY PATH: CreateAsync - Active game exists
    // ---------------------------------------------------
    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenActiveGameAlreadyExists()
    {
        // Arrange
        var db = CreateDbContext();

        db.Games.Add(new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = DateTime.UtcNow,
            Createdat = DateTime.UtcNow,
            Winningnumbers = null // This means active game
        });

        await db.SaveChangesAsync();

        var (service, clock) = CreateService(db);

        var dto = new GameCreateRequestDto
        {
            Weekidentity = new DateTime(2025, 1, 5) // Sunday
        };

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(dto));
    }

    // ----------------------------
    // HAPPY PATH: SetWinningNumbers
    // ----------------------------
    [Fact]
    public async Task SetWinningNumbers_ShouldWork_WhenValidNumbersAndGameExists()
    {
        // Arrange
        var db = CreateDbContext();

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = DateTime.UtcNow,
            Createdat = DateTime.UtcNow,
            Winningnumbers = null
        };

        db.Games.Add(game);
        await db.SaveChangesAsync();

        var (service, clock) = CreateService(db);

        var dto = new GameSetWinnersDto
        {
            WinningNumbers = new() { 3, 7, 12 }
        };

        // Act
        var result = await service.SetWinningNumbersAsync(game.Gameid, dto);

        // Assert
        Assert.Equal(3, result.Winningnumbers!.Count);
        Assert.Contains(7, result.Winningnumbers);
    }

    // ---------------------------------------------------
    // UNHAPPY PATH: SetWinningNumbers - Game not found
    // ---------------------------------------------------
    [Fact]
    public async Task SetWinningNumbers_ShouldThrow_WhenGameDoesNotExist()
    {
        // Arrange
        var db = CreateDbContext();
        var (service, clock) = CreateService(db);

        var dto = new GameSetWinnersDto
        {
            WinningNumbers = new() { 2, 8, 11 }
        };

        // Act + Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            service.SetWinningNumbersAsync(Guid.NewGuid(), dto));
    }

    // ---------------------------------------------------
    // UNHAPPY PATH: Winning numbers already set
    // ---------------------------------------------------
    [Fact]
    public async Task SetWinningNumbers_ShouldThrow_WhenAlreadyHasNumbers()
    {
        // Arrange
        var db = CreateDbContext();

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = DateTime.UtcNow,
            Createdat = DateTime.UtcNow,
            Winningnumbers = new() { 1, 2, 3 }
        };

        db.Games.Add(game);
        await db.SaveChangesAsync();

        var (service, clock) = CreateService(db);

        var dto = new GameSetWinnersDto
        {
            WinningNumbers = new() { 4, 5, 6 }
        };

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.SetWinningNumbersAsync(game.Gameid, dto));
    }
    
    private static (GameService svc, FakeClock clock) CreateService(MyDbContext db, DateTime? nowUtc = null)
    {
        var clock = new FakeClock { UtcNow = nowUtc ?? DateTime.UtcNow };
        return (new GameService(db, clock), clock);
    }
}
