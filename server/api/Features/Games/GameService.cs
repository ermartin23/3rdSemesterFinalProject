using api.Features.Games.Dtos;
using api.Features.Games.Mappings;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Games;

public class GameService : IGameService
{
    private readonly MyDbContext _db;

    public GameService(MyDbContext db)
    {
        _db = db;
    }

    public async Task<List<GameResponseDto>> GetAllAsync()
    {
        var games = await _db.Games
            .AsNoTracking()
            .OrderByDescending(g => g.Createdat)
            .ToListAsync();

        return games.ToGameResponseDtos();
    }

    public async Task<GameResponseDto?> GetByIdAsync(Guid id)
    {
        var game = await _db.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Gameid == id);

        return game?.ToGameResponseDto();
    }

    public async Task<GameResponseDto> CreateAsync(GameCreateRequestDto dto)
    {
        if (dto.Weekidentity.DayOfWeek != DayOfWeek.Sunday)
            throw new InvalidOperationException("Weekidentity must be a Sunday.");

        var active = await _db.Games.FirstOrDefaultAsync(g => g.Winningnumbers == null);
        if (active != null)
            throw new InvalidOperationException("There is already an active game.");

        var dk = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");

        // input as UTC directly
        var weekUtc = DateTime.SpecifyKind(dto.Weekidentity, DateTimeKind.Utc);

        // Convert the week date into Danish local time
        var weekDk = TimeZoneInfo.ConvertTimeFromUtc(weekUtc, dk);

        // Saturday 17:00 in DK - deadline for submitting numbers
        var saturdayDk = weekDk.AddDays(-1).Date.AddHours(17);

        // Convert cutoff back to UTC
        var cutoffUtc = TimeZoneInfo.ConvertTimeToUtc(saturdayDk, dk);

        var cutoff = TimeOnly.FromDateTime(cutoffUtc);

        var game = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = weekUtc,
            Createdat = DateTime.UtcNow,
            Cutofftime = cutoff,
            Winningnumbers = null
        };

        _db.Games.Add(game);
        await _db.SaveChangesAsync();

        return game.ToGameResponseDto();
    }

    public async Task<GameResponseDto> SetWinningNumbersAsync(Guid id, GameSetWinnersDto dto)
    {
        var game = await _db.Games
            .Include(g => g.Boards)
            .ThenInclude(b => b.Player)
            .FirstOrDefaultAsync(g => g.Gameid == id);

        if (game == null)
            throw new KeyNotFoundException($"Game {id} not found.");

        if (game.Winningnumbers != null)
            throw new InvalidOperationException("Winning numbers already set.");

        if (dto.WinningNumbers.Count != 3)
            throw new InvalidOperationException("Exactly 3 winning numbers required.");

        if (dto.WinningNumbers.Distinct().Count() != 3)
            throw new InvalidOperationException("Winning numbers must be unique.");

        if (dto.WinningNumbers.Any(n => n < 1 || n > 16))
            throw new InvalidOperationException("Winning numbers must be between 1 and 16.");

        game.Winningnumbers = dto.WinningNumbers;

        foreach (var board in game.Boards.Where(b => !b.Isdeleted))
        {
            board.Iswinningboard = IsWinningBoard(board, game);
        }

        await _db.SaveChangesAsync();

        await CreateNextWeeklyGameAsync(game.Weekidentity);

        return game.ToGameResponseDto();
    }

    private async Task CreateNextWeeklyGameAsync(DateTime previousWeekSundayUtc)
    {
        var dk = TimeZoneInfo.FindSystemTimeZoneById("Europe/Copenhagen");

        var previousWeekDk = TimeZoneInfo.ConvertTimeFromUtc(previousWeekSundayUtc, dk);

        var nextSundayDk = previousWeekDk.AddDays(7).Date.AddHours(9);

        var nextSundayUtc = TimeZoneInfo.ConvertTimeToUtc(nextSundayDk, dk);

        var nextSaturdayDk = nextSundayDk.AddDays(-1).Date.AddHours(17);
        var nextSaturdayUtc = TimeZoneInfo.ConvertTimeToUtc(nextSaturdayDk, dk);

        var cutoff = TimeOnly.FromDateTime(nextSaturdayUtc);

        var newGame = new Game
        {
            Gameid = Guid.NewGuid(),
            Weekidentity = nextSundayUtc,
            Createdat = DateTime.UtcNow,
            Cutofftime = cutoff,
            Winningnumbers = null
        };

        _db.Games.Add(newGame);
        await _db.SaveChangesAsync();
    }
    
    
    // Jeg er Emre
    private static bool IsWinningBoard(Board board, Game game)
    {
        if (game.Winningnumbers == null || game.Winningnumbers.Count != 3)
            return false;

        if (board.Chosennumbers == null || board.Chosennumbers.Count == 0)
            return false;
        
        // Treat both as sets and check if the board CONTAINS all 3 winning numbers.
        // Order does not matter, and the board may have 5–8 numbers.
        var winningSet = game.Winningnumbers.ToHashSet();
        var boardSet = board.Chosennumbers.ToHashSet();

        return winningSet.All(n => boardSet.Contains(n));
    }

    public async Task<GameDetailsResponseDto?> GetDetailsAsync(Guid id)
    {
        var game = await _db.Games
            .AsNoTracking()
            .Include(g => g.Boards)
            .ThenInclude(b => b.Player)
            .FirstOrDefaultAsync(g => g.Gameid == id && !g.Isdeleted);

        if (game == null)
            return null;

        var activeBoards = game.Boards
            .Where(b => !b.Isdeleted && b.Player != null && !b.Player.Isdeleted)
            .ToList();

        var totalWinningBoards = activeBoards.Count(b => b.Iswinningboard);

        var players = activeBoards
            .GroupBy(b => b.Player)
            .Select(group => new GamePlayerBoardsDto
            {
                PlayerId = group.Key!.Playerid,
                Name = group.Key.Name,
                Phone = group.Key.Phone,
                Email = group.Key.Email,
                Active = group.Key.Active,
                Boards = group.Select(b => new GameBoardSummaryDto
                {
                    BoardId = b.Boardid,
                    PlayerId = b.Playerid,
                    ChosenNumbers = b.Chosennumbers ?? new List<int>(),
                    Price = b.Price,
                    IsWinningBoard = b.Iswinningboard
                }).ToList()
            })
            .ToList();

        var isOpen = game.Winningnumbers == null || game.Winningnumbers.Count == 0;

        return new GameDetailsResponseDto
        {
            GameId = game.Gameid,
            WeekIdentity = game.Weekidentity,
            CreatedAt = game.Createdat,
            CutoffTime = game.Cutofftime,
            WinningNumbers = game.Winningnumbers,
            IsOpen = isOpen,
            TotalWinningBoards = totalWinningBoards,
            Players = players
        };
    }
}
