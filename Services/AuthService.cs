using APPLICATION_BACKEND.Database;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APPLICATION_BACKEND.Services
{
    public class AuthService : IAuthService
    {
        private readonly CampusConnectDbContext _context;
        private readonly IUserService _userService;
        private readonly TokenService _tokenService;
        private readonly IPasswordEncryptionService _passwordEncryptionService;

        public AuthService(CampusConnectDbContext context, IUserService userService, TokenService tokenService, IPasswordEncryptionService passwordEncryptionService)
        {
            _context = context;
            _userService = userService;
            _tokenService = tokenService;
            _passwordEncryptionService = passwordEncryptionService;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                // Verify user credentials
                bool isValid = await _userService.VerifyPasswordAsync(loginRequest.EmailAddress, loginRequest.Password);
                if (!isValid)
                    return null;

                // Get user details
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddres == loginRequest.EmailAddress);

                if (user == null || !user.IsActive)
                    return null;

                // Convert to UserResponseDto
                var userResponse = new UserResponseDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddres,
                    RoleName = user.RoleName,
                    PhoneNumber = user.PhoneNumber,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    IsActive = user.IsActive,
                    DateAdded = user.DateAdded,
                    DateUpdated = user.DateUpdated
                };

                // Generate token
                string token = _tokenService.GenerateToken(userResponse);

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = user.UserId,
                    EmailAddress = user.EmailAddres,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RoleName = user.RoleName,
                    ExpiresAt = DateTime.UtcNow.AddHours(24) // Token expires in 24 hours
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequestDto changePasswordRequest)
        {
            try
            {
                // Verify current password
                bool isCurrentPasswordValid = await _userService.VerifyPasswordAsync(
                    changePasswordRequest.EmailAddress, 
                    changePasswordRequest.CurrentPassword);

                if (!isCurrentPasswordValid)
                    return false;

                // Get user
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddres == changePasswordRequest.EmailAddress);

                if (user == null)
                    return false;

                // Update password
                user.Password = _passwordEncryptionService.EncryptPassword(changePasswordRequest.NewPassword);
                user.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Password change failed: {ex.Message}");
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequest)
        {
            try
            {
                // Check if user exists
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailAddres == forgotPasswordRequest.EmailAddress && u.IsActive);

                if (user == null)
                    return false; // User not found or inactive

                // Update password directly
                user.Password = _passwordEncryptionService.EncryptPassword(forgotPasswordRequest.NewPassword);
                user.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Forgot password failed: {ex.Message}");
            }
        }

        public async Task<AuthResponseDto?> ValidateTokenAsync(string token)
        {
            try
            {
                var authResponse = _tokenService.ValidateToken(token);
                if (authResponse == null)
                    return null;

                // Verify user still exists and is active
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == authResponse.UserId && u.IsActive);

                if (user == null)
                    return null;

                return authResponse;
            }
            catch (Exception ex)
            {
                throw new Exception($"Token validation failed: {ex.Message}");
            }
        }
    }
}
