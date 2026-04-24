using System;
using System.Collections.Generic;

namespace APPLICATION_BACKEND.Database;

public partial class ProductCategoryItem
{
    public long ProductCategoryItemId { get; set; }

    public long ProductCategoryId { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public int Quantity { get; set; }

    public string? ImageUrl { get; set; }

    public int PreperationTimeMinutes { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? DateUpdated { get; set; }
}
