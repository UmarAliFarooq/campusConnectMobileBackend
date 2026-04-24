using APPLICATION_BACKEND.DTOs;

namespace APPLICATION_BACKEND.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<bool> ChangePasswordAsync(ChangePasswordRequestDto changePasswordRequest);
        Task<AuthResponseDto?> ValidateTokenAsync(string token);
        Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequest);
    }
}
