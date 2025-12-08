using api.Features.Boards.Dtos;
using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Boards;

public class BoardService : IBoardService
{
    private readonly MyDbContext _dbContext;

    public BoardService(MyDbContext dbContext)
    {
        _dbContext = dbContext;
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

    public async Task<Board> CreateBoard(CreateBoardRequest request)
    {
        var player = await _dbContext.Players.FindAsync(request.PlayerId);
        var game = await _dbContext.Games.FindAsync(request.GameId);
        var repeatingBoard = request.RepeatingBoardId.HasValue
            ? await _dbContext.Repeatingboards.FindAsync(request.RepeatingBoardId.Value)
            : null;

        if (player == null) throw new ArgumentException("Player does not exist");
        if (game == null) throw new ArgumentException("Game does not exist");

        var board = new Board
        {
            Boardid = Guid.NewGuid(),
            Playerid = request.PlayerId,
            Gameid = request.GameId,
            Chosennumbers = request.ChosenNumbers,
            Iswinningboard = request.IsWinningBoard,
            Price = request.Price,
            Repeatingboardid = request.RepeatingBoardId,

            Player = player,
            Game = game,
            Repeatingboard = repeatingBoard
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
