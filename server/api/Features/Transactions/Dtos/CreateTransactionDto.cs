namespace api;

public class CreateTransactionDto
{
    
    public int Amount { get; set; }
    public string MobilePayTransactionNumber { get; set; } = "";
}