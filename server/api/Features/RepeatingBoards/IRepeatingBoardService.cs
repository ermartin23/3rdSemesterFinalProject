using dataaccess.Entities;

namespace api.Features.RepeatingBoards;

public interface IRepeatingBoardService
{
    Task<Board> ToggleRepeatingBoard(Guid boardId, bool isRepeating);
    Task GenerateBoardsForNewGame(Game newGame);
}
