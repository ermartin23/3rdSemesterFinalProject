namespace api.Features.Boards.Dtos;

public class CreateBoardRequest
{
    public Guid PlayerId { get; set; }
    public Guid GameId { get; set; }
    public List<int> ChosenNumbers { get; set; } = new();
    public bool IsWinningBoard { get; set; }
    public decimal Price { get; set; }
    public Guid? RepeatingBoardId { get; set; }
}