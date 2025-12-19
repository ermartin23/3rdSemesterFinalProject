using api.Features.Boards.Dtos;
using dataaccess.Entities;

namespace api.Features.Boards;

public interface IBoardService
{
    Task<List<Board>> GetAllBoards();
    
    Task<Board?> GetBoardById(Guid id);

    Task<Board?> GetBoardByIdForPlayer(Guid boardId, Guid playerId);
    
    Task<Board> CreateBoardAsync(Guid playerId, CreateBoardRequest request);
    
    Task<Board?> UpdateBoard(Guid id, UpdateBoardRequest dto);
    
    Task<bool> DeleteBoard(Guid id, Guid playerId);
    Task<List<Board>> GetBoardsForPlayerAsync(Guid playerId);

}