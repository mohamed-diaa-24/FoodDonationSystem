using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Restaurant;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDonationSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }


        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        [HttpPost("register")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> RegisterRestaurant([FromBody] CreateRestaurantDto request)
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

            var userId = GetCurrentUserId();
            var result = await _restaurantService.RegisterRestaurantAsync(userId, request);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("my-restaurant")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> GetMyRestaurant()
        {
            var userId = GetCurrentUserId();
            var result = await _restaurantService.GetRestaurantByUserIdAsync(userId);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [HttpPut("my-restaurant")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> UpdateMyRestaurant([FromBody] UpdateRestaurantDto request)
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

            var userId = GetCurrentUserId();
            var result = await _restaurantService.UpdateRestaurantAsync(userId, request);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("nearby")]
        [Authorize(Roles = "Charity,Admin")]
        public async Task<IActionResult> GetNearbyRestaurants(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 10,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _restaurantService.GetNearbyRestaurantsAsync(latitude, longitude, radiusKm, pageNumber, pageSize);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("with-donations")]
        [Authorize(Roles = "Charity,Admin")]
        public async Task<IActionResult> GetRestaurantsWithActiveDonations()
        {
            var result = await _restaurantService.GetRestaurantsWithActiveDonationsAsync();

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRestaurantsForAdmin(
          [FromQuery] int pageNumber = 1,
          [FromQuery] int pageSize = 10,
          [FromQuery] ApprovalStatus? status = null,
          [FromQuery] string? searchTerm = null)
        {
            var result = await _restaurantService.GetRestaurantsForAdminAsync(pageNumber, pageSize, status, searchTerm);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPut("admin/{restaurantId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRestaurantStatus(
            int restaurantId,
            [FromBody] UpdateStatusDto request)
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

            var result = await _restaurantService.UpdateStatusAsync(restaurantId, request.Status, request.RejectionReason);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

    }
}
