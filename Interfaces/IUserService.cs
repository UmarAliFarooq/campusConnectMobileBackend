using APPLICATION_BACKEND.DTOs;

namespace APPLICATION_BACKEND.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(long userId);
        Task<UserResponseDto> CreateUserAsync(UserCreateDto userCreateDto);
        Task<UserResponseDto?> UpdateUserAsync(long userId, UserUpdateDto userUpdateDto);
        Task<bool> DeleteUserAsync(long userId);
        Task<UserResponseDto?> GetUserByEmailAsync(string emailAddress);
        Task<bool> VerifyPasswordAsync(string emailAddress, string password);
    }
}
