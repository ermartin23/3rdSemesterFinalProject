namespace api.Features.RepeatingBoards.Dtos;

public class ToggleRepeatingBoardRequest
{
    public Guid BoardId { get; set; }
    public bool IsRepeating { get; set; }
}