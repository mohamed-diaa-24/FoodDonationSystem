using FoodDonationSystem.Core.DTOs.Charity;
using FoodDonationSystem.Core.DTOs.Common;
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
    public class CharityController : ControllerBase
    {
        private readonly ICharityService _charityService;

        public CharityController(ICharityService charityService)
        {
            _charityService = charityService;
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
        [Authorize(Roles = "Charity")]
        public async Task<IActionResult> RegisterCharity([FromForm] CreateCharityRequest charityRequest, [FromServices] IFileService fileService)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorList = ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                        .ToList();
                    return BadRequest(new ApiResponse<CreateCharityDto>
                    {
                        Errors = errorList,
                        Message = "برجاء ملئ البيانات بشكل صحيح"
                    });
                }

                CreateCharityDto request = charityRequest.ToCreateCharityDto();
                var files = new List<FileUploadItem>
                {
                    new FileUploadItem { File = request.LicenseDocument, Folder = "CharityLicenses" },
                    new FileUploadItem { File = request.ProofDocument, Folder = "CharityProofs" }
                }.Where(f => f.File != null).ToList();

                var savedFiles = await fileService.SaveFilesWithRollbackAsync(files);
                request.LicensePath = savedFiles.ElementAtOrDefault(0);
                request.ProofPath = savedFiles.ElementAtOrDefault(1);

                var userId = GetCurrentUserId();
                var result = await _charityService.RegisterCharityAsync(userId, request);

                if (!result.IsSuccess)
                {
                    fileService.DeleteFiles(savedFiles);
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<CreateCharityDto>
                {
                    Message = "غير مصرح لك بالوصول"
                });
            }
        }

        [HttpGet("my-charity")]
        [Authorize(Roles = "Charity")]
        public async Task<IActionResult> GetMyCharity()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _charityService.GetCharityByUserIdAsync(userId);

                if (result.IsSuccess)
                    return Ok(result);

                return NotFound(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<CharityDto>
                {
                    Message = "غير مصرح لك بالوصول"
                });
            }
        }

        [HttpPut("my-charity")]
        [Authorize(Roles = "Charity")]
        public async Task<IActionResult> UpdateMyCharity([FromBody] UpdateCharityDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorList = ModelState
                        .Where(ms => ms.Value.Errors.Count > 0)
                        .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                        .ToList();
                    return BadRequest(new ApiResponse<UpdateCharityDto>
                    {
                        Errors = errorList,
                        Message = "برجاء ملئ البيانات بشكل صحيح"
                    });
                }

                var userId = GetCurrentUserId();
                var result = await _charityService.UpdateCharityAsync(userId, request);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<CharityDto>
                {
                    Message = "غير مصرح لك بالوصول"
                });
            }
        }

        [HttpGet("nearby")]
        [Authorize(Roles = "Restaurant,Admin")]
        public async Task<IActionResult> GetNearbyCharities(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double radiusKm = 10,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                ApiResponse<PagedResult<CharityDto>> result;

                if (latitude.HasValue && longitude.HasValue && latitude > 0 && longitude > 0)
                {
                    result = await _charityService.GetNearbyCharitiesAsync(
                        latitude.Value, longitude.Value, radiusKm, pageNumber, pageSize);
                }
                else
                {
                    var userId = GetCurrentUserId();
                    result = await _charityService.GetNearbyCharitiesForRestaurantAsync(
                        userId, radiusKm, pageNumber, pageSize);
                }

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new ApiResponse<PagedResult<CharityDto>>
                {
                    Message = "غير مصرح لك بالوصول"
                });
            }
        }

        [HttpGet("by-type/{type}")]
        [Authorize(Roles = "Restaurant,Admin")]
        public async Task<IActionResult> GetCharitiesByType(
            CharityType type)
        {
            var result = await _charityService.GetCharitiesByTypeAsync(type);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("types")]
        [AllowAnonymous]
        public IActionResult GetCharityTypes()
        {
            var charityTypes = Enum.GetValues<CharityType>()
                .Select(ct => new
                {
                    Value = (int)ct,
                    Name = ct.ToString(),
                    DisplayName = ct.ToDisplayName()
                }).ToList();

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Data = charityTypes,
                Message = "تم استرداد أنواع الجمعيات الخيرية بنجاح"
            });
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCharitiesForAdmin(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] ApprovalStatus? status = null,
            [FromQuery] string? searchTerm = null)
        {
            var result = await _charityService.GetCharitiesForAdminAsync(pageNumber, pageSize, status, searchTerm);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPut("admin/{charityId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCharityStatus(
            int charityId,
            [FromBody] UpdateStatusDto request)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(new ApiResponse<UpdateStatusDto>
                {
                    Errors = errorList,
                    Message = "برجاء ملئ البيانات بشكل صحيح"
                });
            }

            var result = await _charityService.UpdateStatusAsync(charityId, request.Status, request.RejectionReason);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpDelete("my-charity")]
        [Authorize(Roles = "Charity")]
        public async Task<IActionResult> DeleteMyCharity()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _charityService.DeleteMyCharityAsync(userId);
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

        [HttpDelete("admin/{charityId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDeleteCharity(int charityId)
        {
            var result = await _charityService.AdminDeleteCharityAsync(charityId);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

    }
}
