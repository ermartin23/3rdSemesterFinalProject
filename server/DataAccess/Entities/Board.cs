using System;
using System.Collections.Generic;

namespace dataaccess.Entities;

public partial class Board
{
    public string Boardid { get; set; } = null!;

    public string Playerid { get; set; } = null!;

    public string Gameid { get; set; } = null!;

    public int? Chosennumbers { get; set; }

    public bool Iswinningboard { get; set; }

    public decimal Price { get; set; }

    public string? Repeatingboardid { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;

    public virtual Repeatingboard? Repeatingboard { get; set; }
}
