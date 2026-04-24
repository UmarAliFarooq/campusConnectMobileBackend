using System.ComponentModel.DataAnnotations;

namespace APPLICATION_BACKEND.DTOs
{
    /// <summary>Shopkeeper advances an order (e.g. Processing → ReadyForDelivery → Delivered).</summary>
    public class ShopkeeperOrderStatusDto
    {
        [Required]
        [StringLength(250)]
        public string NextStatus { get; set; } = string.Empty;
    }
}
