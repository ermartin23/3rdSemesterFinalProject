using System;
using System.Collections.Generic;

namespace dataaccess.Entities;

public partial class Board
{
    public Guid Boardid { get; set; }

    public Guid Playerid { get; set; }

    public Guid Gameid { get; set; }

    public int? Chosennumbers { get; set; }

    public bool Iswinningboard { get; set; }

    public decimal Price { get; set; }

    public Guid? Repeatingboardid { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;

    public virtual Repeatingboard? Repeatingboard { get; set; }
}
