using System;

namespace DataAccess.Models;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PlayerId { get; set; }

    public decimal Amount { get; set; }

    public string MobilePayTransactionNumber { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}