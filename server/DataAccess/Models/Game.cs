using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime GameDate { get; set; } = DateTime.UtcNow;

    // Game status: Open, Closed, Finished
    public string Status { get; set; } = "Open";

    // Numbers drawn during the game
    public List<int> DrawnNumbers { get; set; } = [];

    // Boards participating
    public List<Board> Boards { get; set; } = [];
}