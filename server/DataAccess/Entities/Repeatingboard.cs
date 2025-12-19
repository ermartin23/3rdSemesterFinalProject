using System;
using System.Collections.Generic;

namespace dataaccess.Entities;

public partial class Repeatingboard
{
    public Guid Repeatingboardid { get; set; }

    public Guid Playerid { get; set; }

    public bool Isrepeating { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime? Deletedat { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual Player Player { get; set; } = null!;
}
