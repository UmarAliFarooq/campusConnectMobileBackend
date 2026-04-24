using System.ComponentModel.DataAnnotations;

namespace APPLICATION_BACKEND.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Password { get; set; } = string.Empty;
    }

    public class ChangePasswordRequestDto
    {
        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
