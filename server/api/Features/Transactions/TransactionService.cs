using dataaccess.Entities;
using Infrastructure.Postgres.Scaffolding;
using Microsoft.EntityFrameworkCore;

namespace api;

public class TransactionService : ITransactionService
{
    private readonly MyDbContext _dbContext;
    
    public TransactionService(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Transaction>> GetAllAync()
    {
        return await _dbContext.Transactions
            .Include(t => t.Player)
            .OrderByDescending(t => t.Createdat)
            .ToListAsync();
    }

    public async Task<Transaction> CreatePendingAsync(Guid playerId, int amount, string mobilePayTransactionNumber)
    {
        if (amount == 0)
            throw new ArgumentException("Amount must not be 0", nameof(amount));

        if (string.IsNullOrWhiteSpace(mobilePayTransactionNumber))
            throw new ArgumentException("MobilePay transaction number is required", nameof(mobilePayTransactionNumber));

        var t = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = playerId,
            Amount = amount,
            Mobilepaytransactionnumber = mobilePayTransactionNumber,
            Status = TransactionStatus.Pending.ToString().ToLower(),
            Createdat = DateTime.UtcNow
        };
        
        _dbContext.Transactions.Add(t);
        await _dbContext.SaveChangesAsync();

        return t;
    }

    public async Task ApproveAsync(Guid transactionId)
    {
        var t = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Transactionid == transactionId);
        if (t == null)
            throw new KeyNotFoundException("Transaction not found");

        if (t.Status == TransactionStatus.Approved.ToString())
            return;
        
        if (t.Status == TransactionStatus.Declined.ToString())
            throw new InvalidOperationException("Cannot approve a declined transaction");
        
        var currentBalance = await GetBalanceAsync(t.Playerid);
        var newBalance = currentBalance + t.Amount;
        
        if (newBalance < 0)
            throw new InvalidOperationException("Cannot approve a negative balance");
        
        t.Status = TransactionStatus.Approved.ToString().ToLower();
        await _dbContext.SaveChangesAsync();
    }

    public async Task RejectAsync(Guid transactionId)
    {
        var t = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Transactionid == transactionId);
        if (t == null)
            throw new KeyNotFoundException("Transaction not found");

        if (t.Status == TransactionStatus.Approved.ToString())
            throw new InvalidOperationException("Cannot reject an approved transaction");
        
        t.Status = TransactionStatus.Declined.ToString().ToLower();
        await _dbContext.SaveChangesAsync();
    }

    public async Task<decimal> GetBalanceAsync(Guid playerId)
    {
        var approvedTransactions = await _dbContext.Transactions
            .Where(t =>
                !t.Isdeleted &&
                t.Playerid == playerId &&
                t.Status == TransactionStatus.Approved.ToString().ToLower()
            )
            .SumAsync(t => (decimal)t.Amount);

        var boardCost = await _dbContext.Boards
            .Where(b =>
                !b.Isdeleted &&
                b.Playerid == playerId
            )
            .SumAsync(b => (decimal?)b.Price ?? 0m);
        
        return approvedTransactions - boardCost;
    }
}