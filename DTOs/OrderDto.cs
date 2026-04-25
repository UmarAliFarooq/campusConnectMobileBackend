using System.ComponentModel.DataAnnotations;

namespace APPLICATION_BACKEND.DTOs
{
    public class OrderItemCreateDto
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Product category item ID is required")]
        public long ProductCategoryItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Discount must be non-negative")]
        public int Discount { get; set; } = 0;
    }

    public class OrderItemUpdateDto
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Order item ID is required")]
        public long OrderItemId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int? Quantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Discount must be non-negative")]
        public int? Discount { get; set; }
    }

    public class OrderItemResponseDto
    {
        public long OrderItemId { get; set; }
        public long OrderId { get; set; }
        public long ProductCategoryItemId { get; set; }
        public string ProductCategoryItemName { get; set; } = string.Empty;
        public int ProductCategoryItemPrice { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }
    }

    public class OrderCreateDto
    {
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Customer ID is required")]
        public long CustomerId { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Shopkeeper ID is required")]
        public long? ShopkeeperId { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Deliverer ID is required")]
        public long? DelivererId { get; set; }

        [Required]
        [StringLength(250)]
        public string OrderStatus { get; set; } = "Pending";

        [Required]
        [StringLength(250)]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string PaymentStatus { get; set; } = "Pending";

        [StringLength(250)]
        public string? OrderPickupType { get; set; }

        [StringLength(250)]
        public string? Destination { get; set; }

        [StringLength(250)]
        public string? PickupPoint { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Estimated time must be non-negative")]
        public int? EstimatedTimeMin { get; set; }

        [StringLength(250)]
        public string? SpecialNotes { get; set; }

        [Required]
        public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
    }

    public class OrderUpdateDto
    {
        [Range(1, long.MaxValue, ErrorMessage = "Deliverer ID is required")]
        public long? DelivererId { get; set; }

        [StringLength(250)]
        public string? OrderStatus { get; set; }

        [StringLength(250)]
        public string? PaymentStatus { get; set; }

        [StringLength(250)]
        public string? OrderPickupType { get; set; }

        [StringLength(250)]
        public string? Destination { get; set; }

        [StringLength(250)]
        public string? PickupPoint { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Estimated time must be non-negative")]
        public int? EstimatedTimeMin { get; set; }

        [StringLength(250)]
        public string? SpecialNotes { get; set; }

        public List<OrderItemUpdateDto>? OrderItems { get; set; }
    }

    public class OrderResponseDto
    {
        public long OrderId { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public long? ShopkeeperId { get; set; }
        public string? ShopkeeperName { get; set; }
        public long? DelivererId { get; set; }
        public string? DelivererName { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? OrderPickupType { get; set; }
        public string? Destination { get; set; }
        public string? PickupPoint { get; set; }
        public int? EstimatedTimeMin { get; set; }
        public string? SpecialNotes { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new List<OrderItemResponseDto>();
        public int TotalAmount { get; set; }
        public int DiscountAmount { get; set; }

        /// <summary>Flat delivery fee (PKR) when <see cref="OrderPickupType"/> is Delivery; 0 for pickup.</summary>
        public int DeliveryFee { get; set; }

        /// <summary>Items total minus discounts plus <see cref="DeliveryFee"/>.</summary>
        public int FinalAmount { get; set; }
    }
}
