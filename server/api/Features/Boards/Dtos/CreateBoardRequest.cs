namespace api.Features.Boards.Dtos;

public class CreateBoardRequest
{
    
    public Guid GameId { get; set; }
    public List<int> ChosenNumbers { get; set; } = new();
    public Guid? RepeatingBoardId { get; set; }
}