using System;
using System.Collections.Generic;

namespace APPLICATION_BACKEND.Database;

public partial class User
{
    public long UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string EmailAddres { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? ProfilePictureUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? DateUpdated { get; set; }
}
