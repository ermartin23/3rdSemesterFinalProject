using dataaccess.Entities;

namespace api;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetAllAync();
    Task<Transaction> CreatePendingAsync(Guid playerId, int amount, string mobilePayTransactionNumber);
    Task ApproveAsync(Guid transactionId);
    Task RejectAsync(Guid transactionId);
    Task<decimal> GetBalanceAsync(Guid playerId);
    Task<Transaction?> GetByIdAsync(Guid id);
}