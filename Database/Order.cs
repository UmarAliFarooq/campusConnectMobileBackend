using System;
using System.Collections.Generic;

namespace APPLICATION_BACKEND.Database;

public partial class Order
{
    public long OrderId { get; set; }

    public long CustomerId { get; set; }

    public long? DelivererId { get; set; }

    public long ShopkeeperId { get; set; }

    public string PickupPoint { get; set; } = null!;

    public string? Destination { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string OrderStatus { get; set; } = null!;

    public string? SpecialNotes { get; set; }

    public int EstimatedTimeMin { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string OrderPickupType { get; set; } = null!;
}
