using FoodDonationSystem.Core.DTOs.Auth;
using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Restaurant;
using FoodDonationSystem.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace FoodDonationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState
                  .Where(ms => ms.Value.Errors.Count > 0)
                  .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                  .ToList();
                return BadRequest(new ApiResponse<CreateRestaurantDto>
                {
                    Errors = errorList,
                    Message = "برجاء ملئ البيانات"
                });

            }

            var result = await _authService.RegisterAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState
                  .Where(ms => ms.Value.Errors.Count > 0)
                  .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                  .ToList();
                return BadRequest(new ApiResponse<CreateRestaurantDto>
                {
                    Errors = errorList
                });

            }

            var result = await _authService.LoginAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Get user ID from JWT token
            var userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                await _authService.LogoutAsync(userId);
            }

            return Ok(new { message = "You have successfully logged out" });
        }
    }
}
