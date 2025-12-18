using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Features.RepeatingBoards;

public class RepeatingBoardService : IRepeatingBoardService
{
    private readonly MyDbContext _dbContext;
    private readonly ITransactionService _transactionService;

    public RepeatingBoardService(MyDbContext dbContext, ITransactionService transactionService)
    {
        _dbContext = dbContext;
        _transactionService = transactionService;
    }

    
    public async Task<Board> ToggleRepeatingBoard(Guid playerId, Guid boardId, bool isRepeating)
    {
        var board = await _dbContext.Boards
            .FirstOrDefaultAsync(b => b.Boardid == boardId && !b.Isdeleted);

        if (board == null)
            throw new ArgumentException("Board not found");
        if (board.Playerid != playerId)
            throw new UnauthorizedAccessException("Not your board!");

        if (isRepeating)
        {
            // Find or create the repeatingboard record for this player
            var rb = await _dbContext.Repeatingboards
                .FirstOrDefaultAsync(r => r.Playerid == playerId && !r.Isdeleted);

            if (rb == null)
            {
                rb = new Repeatingboard
                {
                    Repeatingboardid = Guid.NewGuid(),
                    Playerid = playerId,
                    Isrepeating = true,
                    Isdeleted = false,
                    Deletedat = null
                };
                _dbContext.Repeatingboards.Add(rb);
            }
            else
            {
                rb.Isrepeating = true;
            }
            
            board.Repeatingboardid = rb.Repeatingboardid;
        }
        else
        {
            board.Repeatingboardid = null;

            var rb = await _dbContext.Repeatingboards
                .FirstOrDefaultAsync(r => r.Playerid == playerId && !r.Isdeleted);

            if (rb != null)
            {
                var anyLinkedBoards = await _dbContext.Boards.AnyAsync(b =>
                    !b.Isdeleted &&
                    b.Playerid == playerId &&
                    b.Repeatingboardid == rb.Repeatingboardid);

                if (!anyLinkedBoards)
                    rb.Isrepeating = false;
            }
        }

        await _dbContext.SaveChangesAsync();
        return board;
    }


    
    public async Task GenerateBoardsForNewGame(Game newGame)
    {
        var repeatingBoards = await _dbContext.Repeatingboards
            .Where(rb => rb.Isrepeating && !rb.Isdeleted)
            .Include(rb => rb.Player)
            .Include(rb => rb.Boards)
            .ThenInclude(b => b.Game)
            .ToListAsync();

        foreach (var rb in repeatingBoards)
        {
            
            bool exists = await _dbContext.Boards.AnyAsync(b => 
                !b.Isdeleted && 
                b.Repeatingboardid == rb.Repeatingboardid && 
                b.Gameid == newGame.Gameid);
            if (exists) continue;

            var template = rb.Boards
                .Where(b => !b.Isdeleted)
                .OrderByDescending(b => b.Game.Weekidentity)
                .FirstOrDefault();
            if (template == null) continue;

            var balance = await _transactionService.GetBalanceAsync(rb.Playerid);
            if (balance < template.Price)
            {
                continue;
            }
            
            var newBoard = new Board
            {
                Boardid = Guid.NewGuid(),
                Playerid = rb.Playerid,
                Gameid = newGame.Gameid,
                Chosennumbers = template.Chosennumbers,
                Price = template.Price,
                Iswinningboard = false,
                Isdeleted = false,
                Deletedat = null,
                Repeatingboardid = rb.Repeatingboardid
            };

            _dbContext.Boards.Add(newBoard);
        }

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task SetRepeatingForPlayerAsync(Guid playerId, bool isRepeating)
    {
        var rb = await _dbContext.Repeatingboards
            .FirstOrDefaultAsync(r => r.Playerid == playerId && !r.Isdeleted);

        if (rb == null)
            return;

        rb.Isrepeating = isRepeating;
        await _dbContext.SaveChangesAsync();
    }

}
