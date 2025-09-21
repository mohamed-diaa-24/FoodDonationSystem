using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Reservation;

namespace FoodDonationSystem.Core.Interfaces.IServices
{
	public interface IReservationService
	{
		Task<ApiResponse<ReservationDto>> CreateReservationAsync(Guid charityUserId, CreateReservationDto request);
		Task<ApiResponse<bool>> CancelReservationAsync(Guid charityUserId, int reservationId);
		Task<ApiResponse<bool>> CompleteReservationAsync(Guid restaurantUserId, int reservationId);
		Task<ApiResponse<PagedResult<ReservationDto>>> GetMyReservationsAsCharityAsync(Guid charityUserId, int pageNumber = 1, int pageSize = 10);
		Task<ApiResponse<PagedResult<ReservationDto>>> GetReservationsForMyRestaurantAsync(Guid restaurantUserId, int pageNumber = 1, int pageSize = 10);
	}
}
