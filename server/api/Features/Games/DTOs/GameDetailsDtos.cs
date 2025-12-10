

namespace api.Features.Games.Dtos;

public class GameBoardSummaryDto
{
    public Guid BoardId { get; set; }
    public Guid PlayerId { get; set; }

    public List<int> ChosenNumbers { get; set; } = new();
    public decimal Price { get; set; }
    public bool IsWinningBoard { get; set; }
}

public class GamePlayerBoardsDto
{
    public Guid PlayerId { get; set; }

    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Active { get; set; }

    public List<GameBoardSummaryDto> Boards { get; set; } = new();
}

public class GameDetailsResponseDto
{
    public Guid GameId { get; set; }
    public DateTime WeekIdentity { get; set; }
    public DateTime CreatedAt { get; set; }
    public TimeOnly CutoffTime { get; set; }
    public List<int>? WinningNumbers { get; set; }
    public bool IsOpen { get; set; }
    
    public int TotalWinningBoards { get; set; }

    public List<GamePlayerBoardsDto> Players { get; set; } = new();
}