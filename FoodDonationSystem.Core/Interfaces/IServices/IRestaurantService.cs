using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.DTOs.Restaurant;
using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Interfaces.IServices
{
    public interface IRestaurantService
    {
        Task<ApiResponse<RestaurantDto>> RegisterRestaurantAsync(Guid userId, CreateRestaurantDto request);
        Task<ApiResponse<RestaurantDto>> GetRestaurantByUserIdAsync(Guid userId);
        Task<ApiResponse<RestaurantDto>> UpdateRestaurantAsync(Guid userId, UpdateRestaurantDto request);
        Task<ApiResponse<PagedResult<RestaurantDto>>> GetNearbyRestaurantsAsync(double latitude, double longitude, double radiusKm, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PagedResult<RestaurantDto>>> GetNearbyRestaurantsForCharityAsync(Guid charityUserId, double radiusKm, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<IEnumerable<RestaurantDto>>> GetRestaurantsWithActiveDonationsAsync();
        Task<ApiResponse<bool>> UpdateStatusAsync(int restaurantId, ApprovalStatus status, string? rejectionReason = null);
        Task<ApiResponse<PagedResult<RestaurantDto>>> GetRestaurantsForAdminAsync(int pageNumber, int pageSize, ApprovalStatus? status = null, string? searchTerm = null);
        Task<ApiResponse<bool>> DeleteMyRestaurantAsync(Guid userId);
        Task<ApiResponse<bool>> AdminDeleteRestaurantAsync(int restaurantId);
    }
}
