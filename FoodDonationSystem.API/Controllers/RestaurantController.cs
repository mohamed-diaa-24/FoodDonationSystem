using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Restaurant;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Extensions;
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
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("غير مصرح لك بالوصول");
            }
            return Guid.Parse(userIdClaim);
        }

        [HttpPost("register")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> RegisterRestaurant([FromForm] CreateRestaurantRequest restaurantRequest, [FromServices] IFileService fileService)
        {
            try
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
                        Message = "برجاء ملئ البيانات بشكل صحيح"
                    });
                }

                CreateRestaurantDto request = restaurantRequest.ToCreateRestaurantDto();
                var files = new List<FileUploadItem>
                {
                    new FileUploadItem { File = request.LicenseDocument, Folder = "Licenses" },
                    new FileUploadItem { File = request.CommercialRegister, Folder = "Registers" }
                }.Where(f => f.File != null).ToList();

                var savedFiles = await fileService.SaveFilesWithRollbackAsync(files);
                request.LicensePath = savedFiles.ElementAtOrDefault(0);
                request.RegisterPath = savedFiles.ElementAtOrDefault(1);

                var userId = GetCurrentUserId();
                var result = await _restaurantService.RegisterRestaurantAsync(userId, request);

                if (!result.IsSuccess)
                {

                    fileService.DeleteFiles(savedFiles);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<CreateRestaurantDto>
                {
                    Message = "غير مصرح لك بالوصول"
                });
            }
        }

        [HttpGet("my-restaurant")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> GetMyRestaurant()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _restaurantService.GetRestaurantByUserIdAsync(userId);

                if (result.IsSuccess)
                    return Ok(result);

                return NotFound(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<RestaurantDto>
                {
                    Message = "غير مصرح لك بالوصول"
                });
            }
        }

        [HttpPut("my-restaurant")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> UpdateMyRestaurant([FromBody] UpdateRestaurantDto request)
        {
            try
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
                        Message = "برجاء ملئ البيانات بشكل صحيح"
                    });

                }

                var userId = GetCurrentUserId();
                var result = await _restaurantService.UpdateRestaurantAsync(userId, request);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<RestaurantDto>
                {
                    Message = "غير مصرح لك بالوصول"
                });
            }
        }

        [HttpGet("nearby")]
        [Authorize(Roles = "Charity,Admin")]
        public async Task<IActionResult> GetNearbyRestaurants(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double radiusKm = 10,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                ApiResponse<PagedResult<RestaurantDto>> result;

                if (latitude.HasValue && longitude.HasValue && latitude > 0 && longitude > 0)
                {
                    result = await _restaurantService.GetNearbyRestaurantsAsync(
                        latitude.Value, longitude.Value, radiusKm, pageNumber, pageSize);
                }
                else
                {
                    var userId = GetCurrentUserId();
                    result = await _restaurantService.GetNearbyRestaurantsForCharityAsync(
                        userId, radiusKm, pageNumber, pageSize);
                }

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<PagedResult<RestaurantDto>>
                {
                    Message = "غير مصرح لك بالوصول"
                });
            }
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
                    Errors = errorList,
                    Message = "برجاء ملئ البيانات بشكل صحيح"
                });

            }

            var result = await _restaurantService.UpdateStatusAsync(restaurantId, request.Status, request.RejectionReason);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpDelete("my-restaurant")]
        [Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> DeleteMyRestaurant()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _restaurantService.DeleteMyRestaurantAsync(userId);
                if (result.IsSuccess)
                    return Ok(result);
                return BadRequest(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<bool>
                {
                    Message = "غير مصرح لك بالوصول"
                });
            }
        }

        [HttpDelete("admin/{restaurantId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDeleteRestaurant(int restaurantId)
        {
            var result = await _restaurantService.AdminDeleteRestaurantAsync(restaurantId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

    }
}
