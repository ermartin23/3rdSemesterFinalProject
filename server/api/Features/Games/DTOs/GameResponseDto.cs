using System;
using System.Collections.Generic;

namespace api.Features.Games.Dtos
{
    public class GameResponseDto
    {
        public Guid Gameid { get; set; }
        public DateTime Weekidentity { get; set; }
        public DateTime Createdat { get; set; }
        public TimeOnly Cutofftime { get; set; }
        public List<int>? Winningnumbers { get; set; }
        public bool IsOpen { get; set; }

        // NEW:
        public DateTime CutoffUtc { get; set; }
        public bool CanSetWinnersNow { get; set; }
    }
}
