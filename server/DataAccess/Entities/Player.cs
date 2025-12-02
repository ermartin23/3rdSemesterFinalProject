using System;
using System.Collections.Generic;

namespace dataaccess.Entities;

public partial class Player
{
    public Guid Playerid { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime? Deletedat { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual ICollection<Repeatingboard> Repeatingboards { get; set; } = new List<Repeatingboard>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
