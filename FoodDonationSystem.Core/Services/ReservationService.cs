using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Reservation;
using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Extensions;
using FoodDonationSystem.Core.Interfaces;
using FoodDonationSystem.Core.Interfaces.IServices;

namespace FoodDonationSystem.Core.Services
{
	public class ReservationService : IReservationService
	{
		private readonly IUnitOfWork _unitOfWork;

		public ReservationService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResponse<ReservationDto>> CreateReservationAsync(Guid charityUserId, CreateReservationDto request)
		{
			try
			{
				var charity = await _unitOfWork.Charities.GetByUserIdAsync(charityUserId);
				if (charity == null)
					return ApiResponse<ReservationDto>.Failure("لم يتم العثور على الجمعية");

				var donation = await _unitOfWork.Donations.GetByIdAsync(request.DonationId);
				if (donation == null)
					return ApiResponse<ReservationDto>.Failure("لم يتم العثور على التبرع");

				if (donation.Status != DonationStatus.Available || donation.ExpiryDateTime <= DateTime.UtcNow)
					return ApiResponse<ReservationDto>.Failure("التبرع غير متاح للحجز");

				var hasExisting = await _unitOfWork.Reservations.ExistsAsync(r => r.DonationId == donation.Id && r.CharityId == charity.Id && r.Status != ReservationStatus.Cancelled);
				if (hasExisting)
					return ApiResponse<ReservationDto>.Failure("لديك حجز سابق لهذا التبرع");

				var reservation = request.ToEntity(charity.Id);
				await _unitOfWork.Reservations.AddAsync(reservation);

				donation.Status = DonationStatus.Reserved;
				await _unitOfWork.Donations.UpdateAsync(donation);

				await _unitOfWork.SaveChangesAsync();

				// Reload with relations if needed
				var result = reservation.ToDto();
				return ApiResponse<ReservationDto>.Success(result);
			}
			catch (Exception ex)
			{
				return ApiResponse<ReservationDto>.Failure(ex.Message);
			}
		}

		public async Task<ApiResponse<bool>> CancelReservationAsync(Guid charityUserId, int reservationId)
		{
			try
			{
				var charity = await _unitOfWork.Charities.GetByUserIdAsync(charityUserId);
				if (charity == null)
					return ApiResponse<bool>.Failure("لم يتم العثور على الجمعية");

				var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
				if (reservation == null || reservation.CharityId != charity.Id)
					return ApiResponse<bool>.Failure("لم يتم العثور على الحجز");

				if (reservation.Status == ReservationStatus.Cancelled || reservation.Status == ReservationStatus.Completed)
					return ApiResponse<bool>.Failure("لا يمكن إلغاء هذا الحجز");

				reservation.Status = ReservationStatus.Cancelled;
				await _unitOfWork.Reservations.UpdateAsync(reservation);

				var donation = await _unitOfWork.Donations.GetByIdAsync(reservation.DonationId);
				if (donation != null && donation.Status == DonationStatus.Reserved)
				{
					donation.Status = DonationStatus.Available;
					await _unitOfWork.Donations.UpdateAsync(donation);
				}

				await _unitOfWork.SaveChangesAsync();
				return ApiResponse<bool>.Success(true);
			}
			catch (Exception ex)
			{
				return ApiResponse<bool>.Failure(ex.Message);
			}
		}

		public async Task<ApiResponse<bool>> CompleteReservationAsync(Guid restaurantUserId, int reservationId)
		{
			try
			{
				var restaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(restaurantUserId);
				if (restaurant == null)
					return ApiResponse<bool>.Failure("لم يتم العثور على المطعم");

				var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
				if (reservation == null)
					return ApiResponse<bool>.Failure("لم يتم العثور على الحجز");

				var donation = await _unitOfWork.Donations.GetByIdAsync(reservation.DonationId);
				if (donation == null || donation.RestaurantId != restaurant.Id)
					return ApiResponse<bool>.Failure("غير مصرح بإتمام هذا الحجز");

				if (reservation.Status == ReservationStatus.Completed)
					return ApiResponse<bool>.Success(true);

				reservation.Status = ReservationStatus.Completed;
				await _unitOfWork.Reservations.UpdateAsync(reservation);

				donation.Status = DonationStatus.Completed;
				await _unitOfWork.Donations.UpdateAsync(donation);

				await _unitOfWork.SaveChangesAsync();
				return ApiResponse<bool>.Success(true);
			}
			catch (Exception ex)
			{
				return ApiResponse<bool>.Failure(ex.Message);
			}
		}

		public async Task<ApiResponse<PagedResult<ReservationDto>>> GetMyReservationsAsCharityAsync(Guid charityUserId, int pageNumber = 1, int pageSize = 10)
		{
			var charity = await _unitOfWork.Charities.GetByUserIdAsync(charityUserId);
			if (charity == null)
				return ApiResponse<PagedResult<ReservationDto>>.Failure("لم يتم العثور على الجمعية");

			var (items, total) = await _unitOfWork.Reservations.GetPagedAsync(
				pageNumber,
				pageSize,
				r => r.CharityId == charity.Id,
				orderBy: r => r.CreatedAt,
				orderByDescending: true);

			// Note: includes are not handled in generic repo; projection fields may be empty unless loaded elsewhere
			var dto = (items, total).ToPagedResult(pageNumber, pageSize, r => r.ToDto());
			return ApiResponse<PagedResult<ReservationDto>>.Success(dto);
		}

		public async Task<ApiResponse<PagedResult<ReservationDto>>> GetReservationsForMyRestaurantAsync(Guid restaurantUserId, int pageNumber = 1, int pageSize = 10)
		{
			var restaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(restaurantUserId);
			if (restaurant == null)
				return ApiResponse<PagedResult<ReservationDto>>.Failure("لم يتم العثور على المطعم");

			var donationIds = (await _unitOfWork.Donations.FindAsync(d => d.RestaurantId == restaurant.Id)).Select(d => d.Id).ToList();
			if (donationIds.Count == 0)
			{
				return ApiResponse<PagedResult<ReservationDto>>.Success(new PagedResult<ReservationDto>
				{
					Items = new List<ReservationDto>(),
					TotalCount = 0,
					PageNumber = pageNumber,
					PageSize = pageSize
				});
			}

			var (items, total) = await _unitOfWork.Reservations.GetPagedAsync(
				pageNumber,
				pageSize,
				r => donationIds.Contains(r.DonationId),
				orderBy: r => r.CreatedAt,
				orderByDescending: true);

			var dto = (items, total).ToPagedResult(pageNumber, pageSize, r => r.ToDto());
			return ApiResponse<PagedResult<ReservationDto>>.Success(dto);
		}
	}
}
