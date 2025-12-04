using System;
using System.Collections.Generic;

namespace dataaccess.Entities;

public partial class Game
{
    public Guid Gameid { get; set; }

    public DateTime Weekidentity { get; set; }
    
    public List<int>? Winningnumbers { get; set; }

    public TimeOnly Cutofftime { get; set; }

    public DateTime Createdat { get; set; } = DateTime.Now;

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();
}