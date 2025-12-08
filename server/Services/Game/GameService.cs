using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Game
{
    public class GameService : IGameService
    {
        private readonly MyDbContext _context;

        public GameService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            return await _context.Games
                .OrderByDescending(g => g.Createdat)
                .ToListAsync();
        }

        public async Task<Game?> CreateGameAsync(DateTime weekIdentity)
        {
            var now = DateTime.UtcNow;

            // Week identity must be in future
            if (weekIdentity < now)
                return null;

            // There must NOT be an open game
            var anyOpen = await _context.Games
                .AnyAsync(g => IsGameOpen(g.Createdat, g.Cutofftime));

            if (anyOpen)
                return null;

            var newGame = new Game
            {
                Gameid = Guid.NewGuid(),
                Weekidentity = weekIdentity,
                Winningnumbers = new List<int>(),
                Cutofftime = new TimeOnly(17, 0), // default 17:00
                Createdat = now
            };

            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();

            return newGame;
        }

        public async Task<bool> DeleteGameAsync(Guid id)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
                return false;

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return true;
        }

        private bool IsGameOpen(DateTime createdAt, TimeOnly cutoffTime)
        {
            var now = DateTime.UtcNow;

            var cutoff = createdAt.Date
                .AddHours(cutoffTime.Hour)
                .AddMinutes(cutoffTime.Minute);

            return now < cutoff;
        }
    }
}
