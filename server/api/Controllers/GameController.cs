using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Postgres.Scaffolding;
using api.DTOs;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly MyDbContext _context;

        public GameController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGames()
        {
            var games = await _context.Games.ToListAsync();

            var result = games.Select(g => new GameDto
            {
                Gameid = g.Gameid,
                Weekidentity = g.Weekidentity,
                Winningnumbers = g.Winningnumbers,
                Createdat = g.Createdat,
                IsOpen = IsGameOpen(g.Createdat, g.Cutofftime)
            })
            .ToList();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<GameDto>> CreateGame(CreateGameRequest req)
        {
            var now = DateTime.UtcNow;

            if (req.Weekidentity < now)
                return BadRequest("WeekIdentity must be in the future.");

            var open = await _context.Games
                .Where(g => IsGameOpen(g.Createdat, g.Cutofftime))
                .FirstOrDefaultAsync();

            if (open != null)
                return BadRequest("There is already an open game.");

            var newGame = new dataaccess.Entities.Game
            {
                Gameid = Guid.NewGuid(),
                Weekidentity = req.Weekidentity,
                Winningnumbers = new List<int>(),
                Cutofftime = new TimeOnly(17, 0),
                Createdat = now
            };

            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();

            var dto = new GameDto
            {
                Gameid = newGame.Gameid,
                Weekidentity = newGame.Weekidentity,
                Winningnumbers = newGame.Winningnumbers,
                Createdat = newGame.Createdat,
                IsOpen = true
            };

            return Ok(dto);
        }

        [HttpDelete("{gameId:guid}")]
        public async Task<IActionResult> DeleteGame(Guid gameId)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game == null)
                return NotFound();

            _context.Games.Remove(game);
            await _context.SaveChangesAsync();

            return Ok("Game deleted.");
        }

        private bool IsGameOpen(DateTime createdAt, TimeOnly cutoffTime)
        {
            var now = DateTime.UtcNow;

            var cutoff = createdAt.Date.AddHours(cutoffTime.Hour)
                                       .AddMinutes(cutoffTime.Minute);

            return now < cutoff;
        }
    }
}
