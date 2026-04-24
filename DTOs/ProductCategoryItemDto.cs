using System.ComponentModel.DataAnnotations;

namespace APPLICATION_BACKEND.DTOs
{
    public class ProductCategoryItemCreateDto
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int Price { get; set; }

        [Required]
        [Range(0, 9999, ErrorMessage = "Quantity must be non-negative")]
        public int Quantity { get; set; } = 0;

        [Range(0, 480, ErrorMessage = "Preparation time must be non-negative")]
        public int PreperationTimeMinutes { get; set; } = 0;

        public bool IsAvailable { get; set; } = true;

        [StringLength(250)]
        public string? ImageUrl { get; set; }

        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Product category ID is required")]
        public long ProductCategoryId { get; set; }
    }

    public class ProductCategoryItemUpdateDto
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public int? Price { get; set; }

        [Range(0, 9999, ErrorMessage = "Quantity must be non-negative")]
        public int? Quantity { get; set; } = 0;

        [Range(0, 480, ErrorMessage = "Preparation time must be non-negative")]
        public int? PreperationTimeMinutes { get; set; } = 0;

        public bool? IsAvailable { get; set; }

        [StringLength(250)]
        public string? ImageUrl { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Product category ID is required")]
        public long? ProductCategoryId { get; set; }
    }

    public class ProductCategoryItemResponseDto
    {
        public long ProductCategoryItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int PreperationTimeMinutes { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }
        public long ProductCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
