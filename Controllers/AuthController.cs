using Microsoft.AspNetCore.Mvc;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid login data provided");

            try
            {
                var authResponse = await _authService.LoginAsync(loginRequest);
                if (authResponse == null)
                    return ErrorResponse("Invalid email or password");

                return SuccessResponse(authResponse, "Login successful");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Login failed: {ex.Message}");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto changePasswordRequest)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid password change data provided");

            try
            {
                bool success = await _authService.ChangePasswordAsync(changePasswordRequest);
                if (!success)
                    return ErrorResponse("Current password is incorrect or user not found");

                return SuccessResponse(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Password change failed: {ex.Message}");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequest)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid email or password provided");

            try
            {
                bool success = await _authService.ForgotPasswordAsync(forgotPasswordRequest);
                if (!success)
                    return ErrorResponse("User not found or inactive");

                return SuccessResponse(true, "Password updated successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Forgot password failed: {ex.Message}");
            }
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
                return ErrorResponse("Token is required");

            try
            {
                var authResponse = await _authService.ValidateTokenAsync(token);
                if (authResponse == null)
                    return ErrorResponse("Invalid or expired token");

                return SuccessResponse(authResponse, "Token is valid");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"Token validation failed: {ex.Message}");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // For now, this is a simple logout response
            // In a real implementation, you might want to invalidate the token on the server side
            return SuccessResponse(true, "Logout successful");
        }
    }
}
