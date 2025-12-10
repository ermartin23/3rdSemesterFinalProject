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

    
    public async Task<Board> ToggleRepeatingBoard(Guid boardId, bool isRepeating)
    {
        var board = await _dbContext.Boards
            .Include(b => b.Repeatingboard)
            .FirstOrDefaultAsync(b => b.Boardid == boardId);

        if (board == null)
            throw new ArgumentException("Board not found");

        if (board.Repeatingboard == null)
        {
            var repeatingBoard = new Repeatingboard
            {
                Repeatingboardid = Guid.NewGuid(),
                Playerid = board.Playerid,
                Isrepeating = isRepeating
            };
            _dbContext.Repeatingboards.Add(repeatingBoard);

            board.Repeatingboardid = repeatingBoard.Repeatingboardid;
            board.Repeatingboard = repeatingBoard;
        }
        else
        {
            board.Repeatingboard.Isrepeating = isRepeating;
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
            .ToListAsync();

        foreach (var rb in repeatingBoards)
        {
            
            bool exists = await _dbContext.Boards
                .AnyAsync(b => b.Repeatingboardid == rb.Repeatingboardid && b.Gameid == newGame.Gameid);
            if (exists) continue;
            
            var lastBoard = rb.Boards.OrderByDescending(b => b.Boardid).FirstOrDefault();
            if (lastBoard == null) continue;

            int price = lastBoard.Chosennumbers.Count switch
            {
                5 => 20,
                6 => 40,
                7 => 80,
                8 => 160,
                _ => throw new ArgumentException("Invalid number of chosen numbers")
            };

            var balance = await _transactionService.GetBalanceAsync(rb.Playerid);
            if (balance < price)
            {
                continue;
            }

            var transaction = new Transaction
            {
                Transactionid = Guid.NewGuid(),
                Playerid = rb.Playerid,
                Amount = -price,
                Createdat = DateTime.Now
            };
            _dbContext.Transactions.Add(transaction);

            var newBoard = new Board
            {
                Boardid = Guid.NewGuid(),
                Playerid = rb.Playerid,
                Gameid = newGame.Gameid,
                Chosennumbers = lastBoard.Chosennumbers,
                Iswinningboard = false,
                Price = price,
                Repeatingboardid = rb.Repeatingboardid,
                Player = rb.Player,
                Repeatingboard = rb,
                Game = newGame
            };

            _dbContext.Boards.Add(newBoard);
        }

        await _dbContext.SaveChangesAsync();
    }
}
