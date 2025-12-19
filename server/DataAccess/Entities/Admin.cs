using System;
using System.Collections.Generic;

namespace dataaccess.Entities;

public partial class Admin
{
    public Guid Adminid { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime Createdat { get; set; }

    public DateTime Updatedat { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime? Deletedat { get; set; }
}
