using Microsoft.AspNetCore.Mvc;
using APPLICATION_BACKEND.DTOs;

namespace APPLICATION_BACKEND.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult SuccessResponse<T>(T data, string? message = null)
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return Ok(response);
        }

        protected IActionResult ErrorResponse(string error)
        {
            var response = ApiResponse<object>.ErrorResponse(error);
            return Ok(response);
        }
    }
}
