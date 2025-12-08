using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Features.Players.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Players;

public class PlayerService : IPlayerService
{
    private readonly MyDbContext _db;

    public PlayerService(MyDbContext db)
    {
        _db = db;
    }

    public async Task<List<PlayerResponseDto>> GetAllAsync()
    {
        var players = await _db.Players
            .AsNoTracking()
            .Where(p => !p.Isdeleted) // to ignore soft delete playerssss
            .ToListAsync();

        return players.ToPlayerResponseDtos();
    }

    public async Task<PlayerResponseDto?> GetByIdAsync(Guid id)
    {
        var player = await _db.Players
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Playerid == id && !p.Isdeleted);

        return player?.ToPlayerResponseDto();
    }
    
    

    public async Task<PlayerResponseDto> CreateAsync(PlayerCreateRequestDto dto)
    {
        // extra rules you might want:
        // - ensure email is unique
        // - ensure phone is not used twice, etc.
        // you can implement these later
        var now = DateTime.UtcNow;

        var player = new Player
        {
            Playerid = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            Phone = dto.Phone.Trim(),
            Email = dto.Email.Trim().ToLowerInvariant(),
            Active = false, // default inactive
            Createdat = now,
            Updatedat = now,
            Isdeleted = false,
            Deletedat = null
        };

        _db.Players.Add(player);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("DB ERROR: " + ex.InnerException?.Message);
            throw;
        }

        return player.ToPlayerResponseDto();
    }

    public async Task<PlayerResponseDto> UpdateAsync(Guid id, PlayerUpdateRequestDto dto)
    {
        var player = await _db.Players.FirstOrDefaultAsync(p => p.Playerid == id && !p.Isdeleted);
        if (player == null)
        {
            throw new KeyNotFoundException($"Player with id: {id} not found.");
        }

        player.Name = dto.Name.Trim();
        player.Phone = dto.Phone.Trim();
        player.Email = dto.Email.Trim().ToLowerInvariant();
        player.Updatedat = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return player.ToPlayerResponseDto();
    }

    public async Task<PlayerResponseDto> ToggleActiveAsync(Guid id)
    {
        var player = await _db.Players.FirstOrDefaultAsync(p => p.Playerid == id && !p.Isdeleted);
        
        if (player == null)
        {
            throw new KeyNotFoundException($"Player with id {id} not found.");
        }

        player.Active = !player.Active;
        player.Updatedat = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return player.ToPlayerResponseDto();
    }

    public async Task SoftDeleteAsync(Guid id)
    {
        var player = await _db.Players
            .FirstOrDefaultAsync(p => p.Playerid == id && !p.Isdeleted);

        if (player == null)
        {
            throw new KeyNotFoundException($"Player with id {id} not found.");
        }

        var now = DateTime.UtcNow;

        player.Active = false;
        player.Isdeleted = true;
        player.Deletedat = now;
        player.Updatedat = now;

        await _db.SaveChangesAsync();
    }

}