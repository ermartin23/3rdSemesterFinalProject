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
        if (amount <= 0)
            throw new ArgumentException("Amount must not be 0", nameof(amount));

        if (string.IsNullOrWhiteSpace(mobilePayTransactionNumber))
            throw new ArgumentException("MobilePay transaction number is required", nameof(mobilePayTransactionNumber));

        var t = new Transaction
        {
            Transactionid = Guid.NewGuid(),
            Playerid = playerId,
            Amount = amount,
            Mobilepaytransactionnumber = mobilePayTransactionNumber,
            Status = TransactionStatus.Pending.ToString().ToLowerInvariant(),
            Createdat = DateTime.UtcNow,
            Isdeleted = false,
            Deletedat = null
        };

        _dbContext.Transactions.Add(t);
        Console.WriteLine($"Debug before SaveChanges: {t.Transactionid}");
        await _dbContext.SaveChangesAsync();
        Console.WriteLine($"Debug after SaveChanges: {t.Transactionid}");
        return t;
    }

    public async Task ApproveAsync(Guid transactionId)
    {
        var t = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Transactionid == transactionId);
        if (t == null)
            throw new KeyNotFoundException("Transaction not found");
        
        var approved = TransactionStatus.Approved.ToString().ToLowerInvariant();
        var declined  = TransactionStatus.Declined.ToString().ToLowerInvariant();

        if (t.Status == "approved")
            return;
        
        if (t.Status == "declined")
            throw new InvalidOperationException("Cannot approve a declined transaction");
        
        var currentBalance = await GetBalanceAsync(t.Playerid);
        var newBalance = currentBalance + t.Amount;
        
        if (newBalance < 0)
            throw new InvalidOperationException("Cannot approve a negative balance");

        t.Status = approved;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RejectAsync(Guid transactionId)
    {
        var t = await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Transactionid == transactionId);
        if (t == null)
            throw new KeyNotFoundException("Transaction not found");
        
        var approved = TransactionStatus.Approved.ToString().ToLowerInvariant();
        var declined  = TransactionStatus.Declined.ToString().ToLowerInvariant();

        if (t.Status == "approved")
            throw new InvalidOperationException("Cannot reject an approved transaction");
        
        t.Status = declined;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<decimal> GetBalanceAsync(Guid playerId)
    {
        var approvedTransactions = await _dbContext.Transactions
            .Where(t => t.Playerid == playerId &&
                        t.Status == TransactionStatus.Approved.ToString().ToLowerInvariant())
            .SumAsync(t => (decimal)t.Amount);

        var boardCost = await _dbContext.Boards
            .Where(b =>
                !b.Isdeleted &&
                b.Playerid == playerId
            )
            .SumAsync(b => (decimal?)b.Price ?? 0m);
        
        return approvedTransactions - boardCost;
    }

    public Task<Transaction?> GetByIdAsync(Guid id)
    {
        return _dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Transactionid == id);
    }
}