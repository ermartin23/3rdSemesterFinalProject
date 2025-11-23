using System;

namespace DataAccess.Models
{
    public class Transaction
    {
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();
        public string PlayerId { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string MobilepayTransactionNumber { get; set; } = string.Empty;
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Player? Player { get; set; }
    }
}