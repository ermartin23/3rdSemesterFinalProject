using System;

namespace DataAccess.Models
{
    public class Game
    {
        public string GameId { get; set; } = Guid.NewGuid().ToString();
        public DateTime WeekIdentity { get; set; }
        public int[] WinningNumbers { get; set; } = new int[3];
        public TimeOnly CutoffTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}