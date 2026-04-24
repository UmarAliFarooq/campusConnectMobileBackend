using System;
using System.Collections.Generic;

namespace APPLICATION_BACKEND.Database;

public partial class OrderItem
{
    public long OrderItemId { get; set; }

    public long OrderId { get; set; }

    public long ProductCategoryItemId { get; set; }

    public int? Quantity { get; set; }

    public int? UnitPrice { get; set; }

    public int? Discount { get; set; }
}
