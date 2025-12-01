namespace api;

public class CreateTransactionDto
{
    public Guid PlayerId { get; set; }
    public int Amount { get; set; }
    public string MobilePayTransactionNumber { get; set; }
}