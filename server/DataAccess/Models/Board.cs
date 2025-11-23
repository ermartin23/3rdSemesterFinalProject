using System;

namespace DataAccess.Models
{
    public class Board
    {
        public string BoardId { get; set; } = Guid.NewGuid().ToString();
        
        public string PlayerId { get; set; } = string.Empty;
        
        public string GameId { get; set; } = string.Empty;
        
        public int ChosenNumbers { get; set; }
        
        public bool IsWinningBoard { get; set; }
        public decimal Price { get; set; }
        public Player? Player { get; set; }
        public Game? Game { get; set; }
    }
}