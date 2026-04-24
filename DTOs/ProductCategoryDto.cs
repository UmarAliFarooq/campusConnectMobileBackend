using System.ComponentModel.DataAnnotations;

namespace APPLICATION_BACKEND.DTOs
{
    public class ProductCategoryCreateDto
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ProductCategoryUpdateDto
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool? IsActive { get; set; }
    }

    public class ProductCategoryResponseDto
    {
        public long ProductCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
