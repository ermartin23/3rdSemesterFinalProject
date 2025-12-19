using api.Features.Players;
using api.Features.Players.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace tests.ApiTests;

public class PlayerServiceSoftDeleteTests
{
    private readonly IPlayerService _playerService;
    private readonly MyDbContext _db;

    public PlayerServiceSoftDeleteTests(IPlayerService playerService, MyDbContext db)
    {
        _playerService = playerService;
        _db = db;
    }

    [Fact]
    public async Task SoftDeleteAsync_MarksPlayerAsDeleted_AndExcludesFromGetAll()
    {
        
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();
        
        var created = await _playerService.CreateAsync(new PlayerCreateRequestDto
        {
            Name = "Ali Emre",
            Phone = "+45 12 34 56 78",
            Email = "ali@example.com",
            Password = "Test1234!"
        });
        
        var playersBefore = await _playerService.GetAllAsync();
        Assert.Contains(playersBefore, p => p.PlayerId == created.PlayerId);
        
        await _playerService.SoftDeleteAsync(created.PlayerId);
        
        var playersAfter = await _playerService.GetAllAsync();
        Assert.DoesNotContain(playersAfter, p => p.PlayerId == created.PlayerId);
        
        var playerInDb = await _db.Players
            .FirstOrDefaultAsync(p => p.Playerid == created.PlayerId);

        Assert.NotNull(playerInDb);
        Assert.True(playerInDb!.Isdeleted);
        Assert.NotNull(playerInDb.Deletedat);
        Assert.False(playerInDb.Active);
    }
}