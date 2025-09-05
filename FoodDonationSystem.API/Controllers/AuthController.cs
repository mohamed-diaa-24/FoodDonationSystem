using FoodDonationSystem.Core.DTOs.Auth;
using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                return BadRequest(new ApiResponse<RegisterRequestDto>
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
                return BadRequest(new ApiResponse<LoginRequestDto>
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

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ForgetPasswordAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResetPasswordAsync(request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _authService.ChangePasswordAsync(Guid.Parse(userId), request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("send-email-confirmation")]
        public async Task<IActionResult> SendEmailConfirmation([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required");
            }

            var result = await _authService.SendEmailConfirmationAsync(email);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Email and token are required");
            }

            var result = await _authService.ConfirmEmailAsync(email, token);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
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

            return Ok(new { message = "لقد قمت بتسجيل الخروج بنجاح" });
        }
    }
}
