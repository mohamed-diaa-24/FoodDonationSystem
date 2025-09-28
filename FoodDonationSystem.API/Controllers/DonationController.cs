using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Donation;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDonationSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DonationController : ControllerBase
    {
        private readonly IDonationService _donationService;
        private readonly IFileService _fileService;

        public DonationController(IDonationService donationService, IFileService fileService)
        {
            _donationService = donationService;
            _fileService = fileService;
        }

        #region Restaurant Donation Management

        [HttpPost]
        [Authorize(Roles = "Restaurant")]
        public async Task<ActionResult<ApiResponse<DonationDto>>> CreateDonation([FromForm] CreateDonationDto request)
        {
            var userId = GetCurrentUserId();
            var result = await _donationService.CreateDonationAsync(userId, request);
            return Ok(result);
        }


        [HttpGet("my-donations")]
        [Authorize(Roles = "Restaurant")]
        public async Task<ActionResult<ApiResponse<PagedResult<DonationDto>>>> GetMyDonations(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            var result = await _donationService.GetMyDonationsAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("{donationId}")]
        [Authorize(Roles = "Restaurant")]
        public async Task<ActionResult<ApiResponse<DonationDto>>> UpdateDonation(
            int donationId,
            [FromBody] UpdateDonationDto request)
        {
            var userId = GetCurrentUserId();
            var result = await _donationService.UpdateDonationAsync(userId, donationId, request);
            return Ok(result);
        }


        [HttpDelete("{donationId}")]
        [Authorize(Roles = "Restaurant")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteDonation(int donationId)
        {
            var userId = GetCurrentUserId();
            var result = await _donationService.DeleteDonationAsync(userId, donationId);
            return Ok(result);
        }

        #endregion

        #region Charity Management


        [HttpGet("available")]
        [Authorize(Roles = "Charity")]
        public async Task<ActionResult<ApiResponse<PagedResult<DonationDto>>>> GetAvailableDonations(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _donationService.GetAvailableDonationsAsync(pageNumber, pageSize);
            return Ok(result);
        }


        [HttpGet("nearby")]
        [Authorize(Roles = "Charity")]
        public async Task<ActionResult<ApiResponse<PagedResult<DonationDto>>>> GetNearbyDonations(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double radiusKm = 10,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                ApiResponse<PagedResult<DonationDto>> result;

                if (latitude.HasValue && longitude.HasValue && latitude > 0 && longitude > 0)
                {
                    result = await _donationService.GetNearbyDonationsAsync(latitude.Value, longitude.Value, radiusKm, pageNumber, pageSize);
                }
                else
                {
                    var userId = GetCurrentUserId();
                    result = await _donationService.GetNearbyDonationsForCharityAsync(userId, radiusKm, pageNumber, pageSize);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<PagedResult<DonationDto>>.Failure("غير مصرح لك بالوصول"));
            }
        }

        [HttpGet("{donationId}")]
        [Authorize(Roles = "Charity")]
        public async Task<ActionResult<ApiResponse<DonationDto>>> GetDonationById(int donationId)
        {
            var result = await _donationService.GetDonationByIdAsync(donationId);
            return Ok(result);
        }

        #endregion

        #region Image Management
        [HttpDelete("{donationId}/images/{imageId}")]
        [Authorize(Roles = "Restaurant")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveDonationImage(int donationId, int imageId)
        {
            var userId = GetCurrentUserId();
            var result = await _donationService.RemoveDonationImageAsync(userId, donationId, imageId);
            return Ok(result);
        }

        [HttpGet("{donationId}/images")]
        public async Task<ActionResult<ApiResponse<List<DonationImageDto>>>> GetDonationImages(int donationId)
        {
            try
            {
                // Get donation to check if it exists
                var donation = await _donationService.GetDonationByIdAsync(donationId);
                if (!donation.IsSuccess)
                {
                    return NotFound(ApiResponse<List<DonationImageDto>>.Failure("لم يتم العثور على التبرع"));
                }

                // Get images from donation data
                var images = donation.Data.Images.ToList();
                return Ok(ApiResponse<List<DonationImageDto>>.Success(images, "تم استرداد صور التبرع بنجاح"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<List<DonationImageDto>>.Failure($"خطأ في استرداد صور التبرع: {ex.Message}"));
            }
        }

        #endregion

        #region Admin Operations

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PagedResult<DonationDto>>>> GetDonationsForAdmin(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DonationStatus? status = null,
            [FromQuery] string? searchTerm = null)
        {
            var result = await _donationService.GetDonationsForAdminAsync(pageNumber, pageSize, status, searchTerm);
            return Ok(result);
        }
        [HttpPut("admin/{donationId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> AdminUpdateDonationStatus(
            int donationId,
            [FromBody] UpdateDonationStatusDto request)
        {
            var result = await _donationService.AdminUpdateDonationStatusAsync(donationId, request.Status);
            return Ok(result);
        }

        #endregion

        #region Helper Methods

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("غير مصرح لك بالوصول");
            }
            return Guid.Parse(userIdClaim);
        }

        #endregion
    }
}
