using System;
using System.Collections.Generic;

namespace dataaccess.Entities;

public partial class Repeatingboard
{
    public string Repeatingboardid { get; set; } = null!;

    public string Playerid { get; set; } = null!;

    public bool? Isrepeating { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual Player Player { get; set; } = null!;
}
