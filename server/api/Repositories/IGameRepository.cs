using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dataaccess.Entities;

namespace api.Features.Games;

public interface IGameRepository
{
    Task<List<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(Guid id);
    Task<Game?> GetActiveGameAsync();
    Task AddAsync(Game game);
    void Update(Game game);   
    Task SaveChangesAsync();
}