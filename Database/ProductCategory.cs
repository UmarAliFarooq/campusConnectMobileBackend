using System;
using System.Collections.Generic;

namespace APPLICATION_BACKEND.Database;

public partial class ProductCategory
{
    public long ProductCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? DateUpdated { get; set; }
}
