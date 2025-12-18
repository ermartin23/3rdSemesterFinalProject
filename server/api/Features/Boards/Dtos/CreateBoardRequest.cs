namespace api.Features.Boards.Dtos;

public class CreateBoardRequest
{
    
    public Guid GameId { get; set; }
    public List<int> ChosenNumbers { get; set; } = new();
    public Guid? RepeatingBoardId { get; set; }
}

public record BoardResponse(
    Guid BoardId,
    Guid GameId,
    Guid PlayerId,
    List<int> ChosenNumbers,
    decimal Price
);
public record BoardHistoryResponse(
    Guid BoardId,
    Guid GameId,
    DateTime WeekIdentity,
    int Week,
    int Year,
    List<int> ChosenNumbers,
    decimal Price,
    Guid? RepeatingBoardId
);

