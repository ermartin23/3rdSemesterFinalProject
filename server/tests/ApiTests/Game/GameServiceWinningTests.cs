using api.Features.Games;
using api.Features.Games.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace tests.ApiTests;

public class GameServiceWinningTests
{
    private readonly IGameService _gameService;
    private readonly MyDbContext _db;

    public GameServiceWinningTests(IGameService gameService, MyDbContext db)
    {
        _gameService = gameService;
        _db = db;
    }

    [Fact]
    public async Task SetWinningNumbers_MarksWinningBoards_AndDetailsReturnCorrectCounts()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();
        
        var player = new Player
        {
            Playerid = Guid.NewGuid(),
            Name = "Test Player",
            Phone = "+45 11 44 31 13",
            Email = "test@example.com",
            Password = "Test1234!",
            Active = true,
            Createdat = DateTime.UtcNow,
            Updatedat = DateTime.UtcNow,
            Isdeleted = false
        };
        _db.Players.Add(player);

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = DateTime.UtcNow,
            Createdat = DateTime.UtcNow,
            Cutofftime = new TimeOnly(17, 0),
            Winningnumbers = null,
            Isdeleted = false
        };
        _db.Games.Add(game);
        await _db.SaveChangesAsync();
        
        var winningBoard = new Board
        {
            Boardid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Gameid = game.Gameid,
            Chosennumbers = new List<int> { 1, 2, 3, 10, 11 },
            Price = 20m,
            Iswinningboard = false,
            Isdeleted = false
        };

        var losingBoard = new Board
        {
            Boardid = Guid.NewGuid(),
            Playerid = player.Playerid,
            Gameid = game.Gameid,
            Chosennumbers = new List<int> { 1, 4, 5, 6, 7 },
            Price = 20m,
            Iswinningboard = false,
            Isdeleted = false
        };

        _db.Boards.Add(winningBoard);
        _db.Boards.Add(losingBoard);
        await _db.SaveChangesAsync();

        var setWinnersDto = new GameSetWinnersDto
        {
            WinningNumbers = new List<int> { 3, 1, 2 }
        };

        await _gameService.SetWinningNumbersAsync(game.Gameid, setWinnersDto);

        var boardsInDb = await _db.Boards
            .Where(b => b.Gameid == game.Gameid)
            .OrderBy(b => b.Boardid)
            .ToListAsync();

        var winningBoardInDb = boardsInDb.First(b => b.Boardid == winningBoard.Boardid);
        var losingBoardInDb = boardsInDb.First(b => b.Boardid == losingBoard.Boardid);
        
        Assert.True(winningBoardInDb.Iswinningboard);
        Assert.False(losingBoardInDb.Iswinningboard);
        
        var details = await _gameService.GetDetailsAsync(game.Gameid);
        Assert.NotNull(details);
        
        Assert.False(details!.IsOpen);
        
        Assert.Equal(1, details.TotalWinningBoards);
        
        var playerEntry = details.Players.Single(p => p.PlayerId == player.Playerid);

        Assert.Equal(player.Playerid, playerEntry.PlayerId);
        Assert.Equal(2, playerEntry.Boards.Count);

        var winningCountFromDetails = playerEntry.Boards.Count(b => b.IsWinningBoard);
        Assert.Equal(1, winningCountFromDetails);
    }
    
    [Fact]
    public async Task SetWinningNumbers_Twice_ThrowsInvalidOperationException()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = DateTime.UtcNow,
            Createdat = DateTime.UtcNow,
            Cutofftime = new TimeOnly(17, 0),
            Winningnumbers = null,
            Isdeleted = false
        };
        _db.Games.Add(game);
        await _db.SaveChangesAsync();

        var dto = new GameSetWinnersDto
        {
            WinningNumbers = new List<int> { 1, 2, 3 }
        };
        
        await _gameService.SetWinningNumbersAsync(game.Gameid, dto);
        
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _gameService.SetWinningNumbersAsync(game.Gameid, dto);
        });
    }
}