namespace api.Features.Boards.Dtos;

public class UpdateBoardRequest
{
    public Guid? PlayerId { get; set; }
    public Guid? GameId { get; set; }
    public List<int>? ChosenNumbers { get; set; }
    public bool? IsWinningBoard { get; set; }
    public decimal? Price { get; set; }
}