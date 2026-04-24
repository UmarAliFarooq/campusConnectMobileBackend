using Microsoft.AspNetCore.Mvc;
using APPLICATION_BACKEND.DTOs;
using APPLICATION_BACKEND.Interfaces;

namespace APPLICATION_BACKEND.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return SuccessResponse(users, "Users retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving users: {ex.Message}");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(long userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                    return ErrorResponse($"User with ID {userId} not found.");

                return SuccessResponse(user, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving the user: {ex.Message}");
            }
        }

        [HttpGet("email/{emailAddress}")]
        public async Task<IActionResult> GetUserByEmail(string emailAddress)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(emailAddress);
                if (user == null)
                    return ErrorResponse($"User with email {emailAddress} not found.");

                return SuccessResponse(user, "User retrieved successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while retrieving the user: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid user data provided");

            try
            {
                var createdUser = await _userService.CreateUserAsync(userCreateDto);
                return SuccessResponse(createdUser, "User created successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while creating the user: {ex.Message}");
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(long userId, [FromBody] UserUpdateDto userUpdateDto)
        {
            if (!ModelState.IsValid)
                return ErrorResponse("Invalid user data provided");

            try
            {
                var updatedUser = await _userService.UpdateUserAsync(userId, userUpdateDto);
                if (updatedUser == null)
                    return ErrorResponse($"User with ID {userId} not found.");

                return SuccessResponse(updatedUser, "User updated successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while updating the user: {ex.Message}");
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(long userId)
        {
            try
            {
                var deleted = await _userService.DeleteUserAsync(userId);
                if (!deleted)
                    return ErrorResponse($"User with ID {userId} not found.");

                return SuccessResponse(true, "User deleted successfully");
            }
            catch (Exception ex)
            {
                return ErrorResponse($"An error occurred while deleting the user: {ex.Message}");
            }
        }
    }
}
