using System;
using System.Collections.Generic;

namespace APPLICATION_BACKEND.Database;

public partial class SystemRole
{
    public long RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? RoleDescription { get; set; }
}
