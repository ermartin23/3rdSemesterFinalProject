using System;
using System.Collections.Generic;

namespace dataaccess.Entities;

public partial class Transaction
{
    public string Transactionid { get; set; } = null!;

    public string Playerid { get; set; } = null!;

    public int Amount { get; set; }

    public string Mobilepaytransactionnumber { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public virtual Player Player { get; set; } = null!;
}
