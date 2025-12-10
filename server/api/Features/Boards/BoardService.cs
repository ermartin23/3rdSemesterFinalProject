using api.Features.Boards.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Boards;

public class BoardService : IBoardService
{
    private readonly MyDbContext _dbContext;
    private readonly ITransactionService _transactionService;

    public BoardService(MyDbContext dbContext, ITransactionService transactionService)
    {
        _dbContext = dbContext;
        _transactionService = transactionService;
    }

    private int CalculateBoardPrice(int numberCount)
    {
        return numberCount switch
        {
            5 => 20,
            6 => 40,
            7 => 80,
            8 => 160,
            _ => throw new ArgumentException("You must choose between 5 to 8 numbers!")
        };
    }

    public async Task<List<Board>> GetAllBoards()
    {
        return await _dbContext.Boards
            .Where(b => !b.Isdeleted)
            .Include(b => b.Player)
            .Include(b => b.Game)
            .Include(b => b.Repeatingboard)
            .ToListAsync();
    }

    public async Task<Board?> GetBoardById(Guid id)
    {
        return await _dbContext.Boards
            .Include(b => b.Player)
            .Include(b => b.Game)
            .Include(b => b.Repeatingboard)
            .FirstOrDefaultAsync(b => b.Boardid == id && !b.Isdeleted);
    }

    public async Task<Board> CreateBoardAsync(CreateBoardRequest request)
    {
        return await CreateBoardAsync(request.PlayerId, request.ChosenNumbers, request.RepeatingBoardId);
    }
    
    public async Task<Board> CreateBoardAsync(Guid playerId, List<int> chosenNumbers, Guid? repeatingBoardId = null)
    {
    var price = CalculateBoardPrice(chosenNumbers.Count);
        
        var balance = await _transactionService.GetBalanceAsync(playerId);
        if (balance < price)
            throw new InvalidOperationException("Insufficient balance to purchase board.");
        
        var board = new Board
        {
            Boardid = Guid.NewGuid(),
            Playerid = playerId,
            Chosennumbers = chosenNumbers,
            Price = price,
            Repeatingboardid = repeatingBoardId
        };

        _dbContext.Boards.Add(board);
        await _dbContext.SaveChangesAsync();

        return board;
    }

    public async Task<Board?> UpdateBoard(Guid id, UpdateBoardRequest request)
    {
        var board = await _dbContext.Boards.FindAsync(id);
        if (board == null || board.Isdeleted) return null;

        if (request.ChosenNumbers != null)
            board.Chosennumbers = request.ChosenNumbers;

        board.Iswinningboard = request.IsWinningBoard ?? board.Iswinningboard;
        board.Price = request.Price ?? board.Price;

        if (request.PlayerId.HasValue)
        {
            var player = await _dbContext.Players.FindAsync(request.PlayerId.Value);
            if (player == null) throw new ArgumentException("Player does not exist");
            board.Playerid = request.PlayerId.Value;
            board.Player = player;
        }

        if (request.GameId.HasValue)
        {
            var game = await _dbContext.Games.FindAsync(request.GameId.Value);
            if (game == null) throw new ArgumentException("Game does not exist");
            board.Gameid = request.GameId.Value;
            board.Game = game;
        }

        await _dbContext.SaveChangesAsync();
        return board;
    }

    public async Task<bool> DeleteBoard(Guid id)
    {
        var board = await _dbContext.Boards.FindAsync(id);
        if (board == null || board.Isdeleted) return false;

        board.Isdeleted = true;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
