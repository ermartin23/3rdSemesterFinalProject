using api.Features.Boards.Dtos;
using dataaccess.Entities;

namespace api.Features.Boards;

public interface IBoardService
{
    Task<List<Board>> GetAllBoards();

    // Admin usage
    Task<Board?> GetBoardById(Guid id);

    // Player-safe usage
    Task<Board?> GetBoardByIdForPlayer(Guid boardId, Guid playerId);

    // Player creates board for themselves
    Task<Board> CreateBoardAsync(Guid playerId, CreateBoardRequest request);

    // Admin-only update (your controller already enforces this)
    Task<Board?> UpdateBoard(Guid id, UpdateBoardRequest dto);

    // Player deletes only their own
    Task<bool> DeleteBoard(Guid id, Guid playerId);
}