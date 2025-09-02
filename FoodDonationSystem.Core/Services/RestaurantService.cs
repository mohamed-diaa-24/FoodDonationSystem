using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Restaurant;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Extensions;
using FoodDonationSystem.Core.Interfaces;
using FoodDonationSystem.Core.Interfaces.IServices;

namespace FoodDonationSystem.Core.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RestaurantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RestaurantDto>> RegisterRestaurantAsync(Guid userId, CreateRestaurantDto request)
        {
            try
            {
                // Check if user already has a restaurant
                var existingRestaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(userId);
                if (existingRestaurant != null)
                {
                    return ApiResponse<RestaurantDto>.Failure("User already has a restaurant registered");
                }

                // Create new restaurant using extension method
                var restaurant = request.ToEntity(userId);

                await _unitOfWork.Restaurants.AddAsync(restaurant);
                await _unitOfWork.SaveChangesAsync();

                // Get the created restaurant with user info
                var createdRestaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(userId);
                var restaurantDto = createdRestaurant!.ToDto();

                return ApiResponse<RestaurantDto>.Success(restaurantDto, "Restaurant registered successfully. Awaiting admin approval.");
            }
            catch (Exception ex)
            {
                return ApiResponse<RestaurantDto>.Failure($"Error registering restaurant: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RestaurantDto>> GetRestaurantByUserIdAsync(Guid userId)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(userId);
                if (restaurant == null)
                {
                    return ApiResponse<RestaurantDto>.Failure("Restaurant not found");
                }

                var restaurantDto = restaurant.ToDto();
                return ApiResponse<RestaurantDto>.Success(restaurantDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<RestaurantDto>.Failure($"Error retrieving restaurant: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RestaurantDto>> UpdateRestaurantAsync(Guid userId, UpdateRestaurantDto request)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(userId);
                if (restaurant == null)
                {
                    return ApiResponse<RestaurantDto>.Failure("Restaurant not found");
                }

                // Update restaurant using extension method
                restaurant.UpdateFromDto(request);

                await _unitOfWork.Restaurants.UpdateAsync(restaurant);
                await _unitOfWork.SaveChangesAsync();

                var restaurantDto = restaurant.ToDto();
                return ApiResponse<RestaurantDto>.Success(restaurantDto, "Restaurant updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<RestaurantDto>.Failure($"Error updating restaurant: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<RestaurantDto>>> GetNearbyRestaurantsAsync(
           double latitude, double longitude, double radiusKm, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var restaurants = await _unitOfWork.Restaurants.GetNearbyRestaurantsAsync(latitude, longitude, radiusKm);

                // Use extension method for manual pagination
                var result = restaurants.ToManualPagedResult(pageNumber, pageSize, r => r.ToDto());

                return ApiResponse<PagedResult<RestaurantDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<RestaurantDto>>.Failure($"Error retrieving nearby restaurants: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<RestaurantDto>>> GetRestaurantsWithActiveDonationsAsync()
        {
            try
            {
                var restaurants = await _unitOfWork.Restaurants.GetRestaurantsWithActiveDonationsAsync();
                var restaurantDtos = restaurants.ToDto();

                return ApiResponse<IEnumerable<RestaurantDto>>.Success(restaurantDtos);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RestaurantDto>>.Failure($"Error retrieving restaurants with donations: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int restaurantId, ApprovalStatus status, string? rejectionReason = null)
        {
            try
            {
                var result = await _unitOfWork.Restaurants.UpdateStatusAsync(restaurantId, status, rejectionReason);
                if (!result)
                {
                    return ApiResponse<bool>.Failure("Restaurant not found");
                }

                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "Restaurant status updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"Error updating restaurant status: {ex.Message}");
            }
        }






        public async Task<ApiResponse<PagedResult<RestaurantDto>>> GetRestaurantsForAdminAsync(
            int pageNumber, int pageSize, ApprovalStatus? status = null, string? searchTerm = null)
        {
            try
            {
                var restaurantsResult = await _unitOfWork.Restaurants.GetRestaurantsForAdminAsync(
                    pageNumber, pageSize, status, searchTerm);

                // Use extension method for mapping
                var result = restaurantsResult.ToRestaurantPagedResult(pageNumber, pageSize);

                return ApiResponse<PagedResult<RestaurantDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<RestaurantDto>>.Failure($"Error retrieving restaurants: {ex.Message}");
            }
        }
    }
}
