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
            return Guid.Parse(userIdClaim!);
        }

        [HttpPost("register")]
        [Authorize(Roles = "Charity")]
        public async Task<IActionResult> RegisterCharity([FromForm] CreateCharityRequest charityRequest, [FromServices] IFileService fileService)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(new ApiResponse<CreateCharityDto>
                {
                    Errors = errorList
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

        [HttpGet("my-charity")]
        [Authorize(Roles = "Charity")]
        public async Task<IActionResult> GetMyCharity()
        {
            var userId = GetCurrentUserId();
            var result = await _charityService.GetCharityByUserIdAsync(userId);

            if (result.IsSuccess)
                return Ok(result);

            return NotFound(result);
        }

        [HttpPut("my-charity")]
        [Authorize(Roles = "Charity")]
        public async Task<IActionResult> UpdateMyCharity([FromBody] UpdateCharityDto request)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(new ApiResponse<UpdateCharityDto>
                {
                    Errors = errorList
                });
            }

            var userId = GetCurrentUserId();
            var result = await _charityService.UpdateCharityAsync(userId, request);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("nearby")]
        [Authorize(Roles = "Restaurant,Admin")]
        public async Task<IActionResult> GetNearbyCharities(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 10,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            ApiResponse<PagedResult<CharityDto>> result;
            if (latitude > 0 && longitude > 0)
            {
                result = await _charityService.GetNearbyCharitiesAsync(latitude, longitude, radiusKm, pageNumber, pageSize);
            }
            else
            {
                //logic 
                result = await _charityService.GetNearbyCharitiesAsync(latitude, longitude, radiusKm, pageNumber, pageSize);
            }

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
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
                Message = "Charity types retrieved successfully"
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
                    Errors = errorList
                });
            }

            var result = await _charityService.UpdateStatusAsync(charityId, request.Status, request.RejectionReason);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }




    }
}
