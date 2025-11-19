using System;

namespace Server.DataAccess.Models; 


public class Transaction
{
    public Guid Id { get; set; }  // GUID id of the transaction

    public Guid PlayerId { get; set; }  // References the Player who owns this transaction

    public decimal Amount { get; set; }  // Amount of money in the transaction

    public string MobilePayTransactionNumber { get; set; } = string.Empty;  // MobilePay reference number

    public string Status { get; set; } = "Pending";  // Pending, Completed, Failed, Refunded...

    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // When the transaction was created
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  // When the transaction was last updated
}