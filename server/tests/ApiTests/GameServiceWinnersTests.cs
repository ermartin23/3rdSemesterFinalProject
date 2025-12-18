using api.Helpers.Time;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Features.Auth;
using api.Features.Games;
using api.Features.Games.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Xunit;
using api.Features.RepeatingBoards;


namespace tests.ApiTests;


public class GameServiceWinnersTests
{
    private static MyDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MyDbContext(options);
    }

    // Sunday 2025-12-21 00:00:00Z -> cutoff is Sat 17:00 DK = 16:00 UTC (December = CET)
    private static DateTime WeekSundayUtc => new DateTime(2025, 12, 21, 0, 0, 0, DateTimeKind.Utc);
    private static DateTime CutoffUtc_Expected => new DateTime(2025, 12, 20, 16, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task SetWinningNumbers_BeforeCutoff_ShouldThrow()
    {
        using var db = CreateDb();

        var clock = new FakeClock { UtcNow = CutoffUtc_Expected.AddMinutes(-1) };
        var svc = new GameService(db, clock, new NoopRepeatingBoardService());

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = WeekSundayUtc,
            Createdat = WeekSundayUtc.AddDays(-2),
            Cutofftime = TimeOnly.FromDateTime(CutoffUtc_Expected),
            Winningnumbers = null,
            Isdeleted = false
        };

        db.Games.Add(game);
        await db.SaveChangesAsync();

        var dto = new GameSetWinnersDto { WinningNumbers = new List<int> { 1, 2, 3 } };

        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.SetWinningNumbersAsync(game.Gameid, dto));
    }

    [Fact]
    public async Task SetWinningNumbers_AfterCutoff_ShouldSetWinners_AndCreateNextWeekGame()
    {
        using var db = CreateDb();

        var clock = new FakeClock { UtcNow = CutoffUtc_Expected.AddMinutes(1) };
        var svc = new GameService(db, clock, new NoopRepeatingBoardService());

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = WeekSundayUtc,
            Createdat = WeekSundayUtc.AddDays(-2),
            Cutofftime = TimeOnly.FromDateTime(CutoffUtc_Expected),
            Winningnumbers = null,
            Isdeleted = false,
            Boards = new List<Board>()
        };

        db.Games.Add(game);
        await db.SaveChangesAsync();

        var dto = new GameSetWinnersDto { WinningNumbers = new List<int> { 1, 2, 3 } };

        var updated = await svc.SetWinningNumbersAsync(game.Gameid, dto);

        Assert.NotNull(updated.Winningnumbers);
        Assert.Equal(new[] { 1, 2, 3 }, updated.Winningnumbers);

        // next week created
        var nextWeek = WeekSundayUtc.AddDays(7);
        var exists = await db.Games.AnyAsync(g => g.Weekidentity == nextWeek && !g.Isdeleted);
        Assert.True(exists);
    }

    [Fact]
    public async Task SetWinningNumbers_ShouldMarkWinningBoards()
    {
        using var db = CreateDb();

        var clock = new FakeClock { UtcNow = CutoffUtc_Expected.AddMinutes(1) };
        var svc = new GameService(db, clock, new NoopRepeatingBoardService());
        var passwordService = new PasswordService();

        var player = new Player
        {
            Playerid = Guid.NewGuid(),
            Name = "Test",
            Phone = "123",
            Email = "t@t.com",
            Password = passwordService.Hash("Test123!"), 
            Active = true,
            Createdat = DateTime.UtcNow,
            Updatedat = DateTime.UtcNow,
            Isdeleted = false
        };

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = WeekSundayUtc,
            Createdat = WeekSundayUtc.AddDays(-2),
            Cutofftime = TimeOnly.FromDateTime(CutoffUtc_Expected),
            Winningnumbers = null,
            Isdeleted = false,
            Boards = new List<Board>()
        };

        var winningBoard = new Board
        {
            Boardid = Guid.NewGuid(),
            Gameid = game.Gameid,
            Playerid = player.Playerid,
            Chosennumbers = new List<int> { 1, 2, 3, 10, 11 },
            Price = 50,
            Isdeleted = false,
            Iswinningboard = false,
            Player = player
        };

        var losingBoard = new Board
        {
            Boardid = Guid.NewGuid(),
            Gameid = game.Gameid,
            Playerid = player.Playerid,
            Chosennumbers = new List<int> { 1, 2, 9, 10, 11 },
            Price = 50,
            Isdeleted = false,
            Iswinningboard = false,
            Player = player
        };

        game.Boards.Add(winningBoard);
        game.Boards.Add(losingBoard);

        db.Players.Add(player);
        db.Games.Add(game);
        db.Boards.AddRange(winningBoard, losingBoard);
        await db.SaveChangesAsync();

        var dto = new GameSetWinnersDto { WinningNumbers = new List<int> { 1, 2, 3 } };

        await svc.SetWinningNumbersAsync(game.Gameid, dto);

        var boards = await db.Boards.ToListAsync();
        Assert.True(boards.Find(b => b.Boardid == winningBoard.Boardid)!.Iswinningboard);
        Assert.False(boards.Find(b => b.Boardid == losingBoard.Boardid)!.Iswinningboard);
    }

    [Fact]
    public async Task SetWinningNumbers_WithDuplicates_ShouldThrow()
    {
        using var db = CreateDb();

        var clock = new FakeClock { UtcNow = CutoffUtc_Expected.AddMinutes(1) };
        var svc = new GameService(db, clock, new NoopRepeatingBoardService());


        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = WeekSundayUtc,
            Createdat = WeekSundayUtc.AddDays(-2),
            Cutofftime = TimeOnly.FromDateTime(CutoffUtc_Expected),
            Winningnumbers = null,
            Isdeleted = false
        };

        db.Games.Add(game);
        await db.SaveChangesAsync();

        var dto = new GameSetWinnersDto { WinningNumbers = new List<int> { 1, 1, 2 } };

        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.SetWinningNumbersAsync(game.Gameid, dto));
    }
    
    public class NoopRepeatingBoardService : IRepeatingBoardService
    {
        public Task<Board> ToggleRepeatingBoard(Guid playerId, Guid boardId, bool isRepeating)
            => throw new NotImplementedException();

        public Task GenerateBoardsForNewGame(Game newGame)
            => Task.CompletedTask;

        public Task SetRepeatingForPlayerAsync(Guid playerId, bool isRepeating)
            => Task.CompletedTask;
    }

}