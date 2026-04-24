using System.ComponentModel.DataAnnotations;

namespace APPLICATION_BACKEND.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        [StringLength(250)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(250)]
        public string? RoleName { get; set; }

        [StringLength(250)]
        public string? PhoneNumber { get; set; }

        [StringLength(250)]
        public string? ProfilePictureUrl { get; set; }

        public bool? IsActive { get; set; }
    }
}
