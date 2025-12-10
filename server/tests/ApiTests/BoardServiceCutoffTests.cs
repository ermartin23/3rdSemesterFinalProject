using System;
using System.Threading.Tasks;
using api.Features.Boards;
using api.Features.Boards.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace tests.ApiTests;

public class BoardServiceCutoffTests
{
    private MyDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MyDbContext(options);
    }

    [Fact]
    public async Task CreateBoard_ShouldThrow_WhenAfterCutoffTime()
    {
        // Arrange
        var db = CreateDbContext();

        // A Sunday game
        var sundayUtc = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc);

        // Cutoff = 4 Jan 2025 - 16:00 UTC
        var cutoffUtc = new DateTime(2025, 1, 4, 16, 0, 0, DateTimeKind.Utc);

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = sundayUtc,
            Createdat = DateTime.UtcNow,
            Cutofftime = TimeOnly.FromDateTime(cutoffUtc),
            Winningnumbers = null
        };

        db.Games.Add(game);
        await db.SaveChangesAsync();

        var service = new BoardService(db);

        var request = new CreateBoardRequest
        {
            PlayerId = Guid.NewGuid(),
            GameId = game.Gameid,
            ChosenNumbers = new() { 1, 5, 9 }
        };

        // Simulate NOW > cutoff
        var now = cutoffUtc.AddHours(2); // 2 hours after deadline
        
    }
}