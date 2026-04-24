using Microsoft.EntityFrameworkCore;
using APPLICATION_BACKEND.Database;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Services
{
    public class UserService : IUserService
    {
        private readonly CampusConnectDbContext _context;
        private readonly IPasswordEncryptionService _passwordEncryptionService;

        public UserService(CampusConnectDbContext context, IPasswordEncryptionService passwordEncryptionService)
        {
            _context = context;
            _passwordEncryptionService = passwordEncryptionService;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(MapToUserResponseDto);
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(long userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user != null ? MapToUserResponseDto(user) : null;
        }

        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto userCreateDto)
        {
            var user = new User
            {
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                EmailAddres = userCreateDto.EmailAddress,
                Password = _passwordEncryptionService.EncryptPassword(userCreateDto.Password),
                RoleName = userCreateDto.RoleName,
                PhoneNumber = userCreateDto.PhoneNumber,
                ProfilePictureUrl = userCreateDto.ProfilePictureUrl,
                IsActive = true,
                DateAdded = DateTime.UtcNow,
                DateUpdated = null
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToUserResponseDto(user);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(long userId, UserUpdateDto userUpdateDto)
        {
            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null)
                return null;

            if (!string.IsNullOrEmpty(userUpdateDto.FirstName))
                existingUser.FirstName = userUpdateDto.FirstName;

            if (!string.IsNullOrEmpty(userUpdateDto.LastName))
                existingUser.LastName = userUpdateDto.LastName;

            if (!string.IsNullOrEmpty(userUpdateDto.RoleName))
                existingUser.RoleName = userUpdateDto.RoleName;

            if (!string.IsNullOrEmpty(userUpdateDto.PhoneNumber))
                existingUser.PhoneNumber = userUpdateDto.PhoneNumber;

            if (userUpdateDto.ProfilePictureUrl != null)
                existingUser.ProfilePictureUrl = userUpdateDto.ProfilePictureUrl;

            if (userUpdateDto.IsActive.HasValue)
                existingUser.IsActive = userUpdateDto.IsActive.Value;

            existingUser.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToUserResponseDto(existingUser);
        }

        public async Task<bool> DeleteUserAsync(long userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserResponseDto?> GetUserByEmailAsync(string emailAddress)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddres == emailAddress);
            
            return user != null ? MapToUserResponseDto(user) : null;
        }

        public async Task<bool> VerifyPasswordAsync(string emailAddress, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddres == emailAddress);
            
            if (user == null)
                return false;

            return _passwordEncryptionService.VerifyPassword(password, user.Password);
        }

        private static UserResponseDto MapToUserResponseDto(User user)
        {
            return new UserResponseDto
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
        }
    }
}
