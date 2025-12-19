using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Games;

public class GameRepository : IGameRepository
{
    private readonly MyDbContext _db;

    public GameRepository(MyDbContext db)
    {
        _db = db;
    }

    public Task<List<Game>> GetAllAsync()
    {
        return _db.Games
            .AsNoTracking()
            .OrderByDescending(g => g.Createdat)
            .ToListAsync();
    }

    public Task<Game?> GetByIdAsync(Guid id)
    {
        return _db.Games.FirstOrDefaultAsync(g => g.Gameid == id);
    }

    public Task<Game?> GetActiveGameAsync()
    {
        return _db.Games.FirstOrDefaultAsync(g => g.Winningnumbers == null);
    }

    public async Task AddAsync(Game game)
    {
        await _db.Games.AddAsync(game);
    }
    
    public void Update(Game game)
    {
        _db.Games.Update(game);
    }

    public Task SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }
}