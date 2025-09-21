using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Reservation;
using FoodDonationSystem.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodDonationSystem.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class ReservationController : ControllerBase
	{
		private readonly IReservationService _reservationService;

		public ReservationController(IReservationService reservationService)
		{
			_reservationService = reservationService;
		}

		[HttpPost]
		[Authorize(Roles = "Charity")]
		public async Task<ActionResult<ApiResponse<ReservationDto>>> Create([FromBody] CreateReservationDto request)
		{
			var userId = GetCurrentUserId();
			var result = await _reservationService.CreateReservationAsync(userId, request);
			return Ok(result);
		}

		[HttpDelete("{reservationId}")]
		[Authorize(Roles = "Charity")]
		public async Task<ActionResult<ApiResponse<bool>>> Cancel(int reservationId)
		{
			var userId = GetCurrentUserId();
			var result = await _reservationService.CancelReservationAsync(userId, reservationId);
			return Ok(result);
		}

		[HttpGet("my-reservations")]
		[Authorize(Roles = "Charity")]
		public async Task<ActionResult<ApiResponse<PagedResult<ReservationDto>>>> GetMyReservations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			var userId = GetCurrentUserId();
			var result = await _reservationService.GetMyReservationsAsCharityAsync(userId, pageNumber, pageSize);
			return Ok(result);
		}

		[HttpGet("restaurant-reservations")]
		[Authorize(Roles = "Restaurant")]
		public async Task<ActionResult<ApiResponse<PagedResult<ReservationDto>>>> GetRestaurantReservations([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			var userId = GetCurrentUserId();
			var result = await _reservationService.GetReservationsForMyRestaurantAsync(userId, pageNumber, pageSize);
			return Ok(result);
		}

		[HttpPut("{reservationId}/complete")]
		[Authorize(Roles = "Restaurant")]
		public async Task<ActionResult<ApiResponse<bool>>> Complete(int reservationId)
		{
			var userId = GetCurrentUserId();
			var result = await _reservationService.CompleteReservationAsync(userId, reservationId);
			return Ok(result);
		}

		private Guid GetCurrentUserId()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			return Guid.Parse(userId!);
		}
	}
}
