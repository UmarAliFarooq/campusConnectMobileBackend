using System.ComponentModel.DataAnnotations;

namespace APPLICATION_BACKEND.DTOs
{
    public class UserCreateDto
    {
        [Required]
        [StringLength(250)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string RoleName { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(250)]
        public string? ProfilePictureUrl { get; set; }
    }
}
