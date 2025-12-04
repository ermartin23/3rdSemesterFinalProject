using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // Treat input as UTC explicitly
        var weekUtc = DateTime.SpecifyKind(dto.Weekidentity, DateTimeKind.Utc);

        // Convert the week date into Danish local time
        var weekDk = TimeZoneInfo.ConvertTimeFromUtc(weekUtc, dk);

        // Saturday 17:00 in DK
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
        var game = await _db.Games.FirstOrDefaultAsync(g => g.Gameid == id);

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
}
