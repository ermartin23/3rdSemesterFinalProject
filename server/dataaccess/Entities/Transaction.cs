using System;
using System.Collections.Generic;

namespace dataaccess.Entities;

public partial class Transaction
{
    public Guid Transactionid { get; set; }

    public Guid Playerid { get; set; }

    public int Amount { get; set; }

    public string Mobilepaytransactionnumber { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public virtual Player Player { get; set; } = null!;
}
