using dataaccess.Entities;

namespace api.Services.Game
{
    public interface IGameService
    {
        Task<IEnumerable<Game>> GetAllGamesAsync();
        Task<Game?> CreateGameAsync(DateTime weekIdentity);
        Task<bool> DeleteGameAsync(Guid id);
    }
}