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
                    return ApiResponse<RestaurantDto>.Failure("المستخدم لديه مطعم مسجل بالفعل");
                }

                // Create new restaurant using extension method
                var restaurant = request.ToEntity(userId);
                restaurant.LicenseDocument = request.LicensePath;
                restaurant.CommercialRegister = request.RegisterPath;

                await _unitOfWork.Restaurants.AddAsync(restaurant);
                await _unitOfWork.SaveChangesAsync();

                // Get the created restaurant with user info
                var createdRestaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(userId);
                var restaurantDto = createdRestaurant!.ToDto();

                return ApiResponse<RestaurantDto>.Success(restaurantDto, "تم تسجيل المطعم بنجاح. بانتظار موافقة الإدارة");
            }
            catch (Exception ex)
            {
                return ApiResponse<RestaurantDto>.Failure($"خطأ في تسجيل المطعم: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RestaurantDto>> GetRestaurantByUserIdAsync(Guid userId)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(userId);
                if (restaurant == null)
                {
                    return ApiResponse<RestaurantDto>.Failure("لم يتم العثور على المطعم");
                }

                var restaurantDto = restaurant.ToDto();
                return ApiResponse<RestaurantDto>.Success(restaurantDto, "تم استرداد بيانات المطعم بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<RestaurantDto>.Failure($"خطأ في استرداد المطعم: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RestaurantDto>> UpdateRestaurantAsync(Guid userId, UpdateRestaurantDto request)
        {
            try
            {
                var restaurant = await _unitOfWork.Restaurants.GetByUserIdAsync(userId);
                if (restaurant == null)
                {
                    return ApiResponse<RestaurantDto>.Failure("لم يتم العثور على المطعم");
                }

                // Update restaurant using extension method
                restaurant.UpdateFromDto(request);

                await _unitOfWork.Restaurants.UpdateAsync(restaurant);
                await _unitOfWork.SaveChangesAsync();

                var restaurantDto = restaurant.ToDto();
                return ApiResponse<RestaurantDto>.Success(restaurantDto, "تم تحديث المطعم بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<RestaurantDto>.Failure($"حدث خطأ أثناء تحديث المطعم: {ex.Message}");
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

                return ApiResponse<PagedResult<RestaurantDto>>.Success(result, "تم استرداد المطاعم القريبة بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<RestaurantDto>>.Failure($"حدث خطأ أثناء استرداد المطاعم القريبة: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<RestaurantDto>>> GetRestaurantsWithActiveDonationsAsync()
        {
            try
            {
                var restaurants = await _unitOfWork.Restaurants.GetRestaurantsWithActiveDonationsAsync();
                var restaurantDtos = restaurants.ToDto();

                return ApiResponse<IEnumerable<RestaurantDto>>.Success(restaurantDtos, "تم استرداد المطاعم التي تحتوي على تبرعات بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<RestaurantDto>>.Failure($"حدث خطأ أثناء استرداد المطاعم التي تحتوي على تبرعات: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> UpdateStatusAsync(int restaurantId, ApprovalStatus status, string? rejectionReason = null)
        {
            try
            {
                var result = await _unitOfWork.Restaurants.UpdateStatusAsync(restaurantId, status, rejectionReason);
                if (!result)
                {
                    return ApiResponse<bool>.Failure("لم يتم العثور على المطعم");
                }

                await _unitOfWork.SaveChangesAsync();
                return ApiResponse<bool>.Success(true, "تم تحديث حالة المطعم بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Failure($"حدث خطأ أثناء تحديث حالة المطعم: {ex.Message}");
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

                return ApiResponse<PagedResult<RestaurantDto>>.Success(result, "تم استرداد قائمة المطاعم بنجاح");
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResult<RestaurantDto>>.Failure($"خطأ في استرداد المطاعم: {ex.Message}");
            }
        }
    }
}
