using api.Features.Boards.Dtos;
using dataaccess.Entities;

namespace api.Features.Boards;

public interface IBoardService
{
    Task<List<Board>> GetAllBoards();
    Task<Board?> GetBoardById(Guid id);
    Task<Board> CreateBoard(CreateBoardRequest dto);
    Task<Board?> UpdateBoard(Guid id, UpdateBoardRequest dto);
    Task<bool> DeleteBoard(Guid id);
}