using FoodDonationSystem.Core.DTOs.Charity;
using FoodDonationSystem.Core.DTOs.Common;
using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Interfaces.IServices
{
    public interface ICharityService
    {
        Task<ApiResponse<CharityDto>> RegisterCharityAsync(Guid userId, CreateCharityDto request);
        Task<ApiResponse<CharityDto>> GetCharityByUserIdAsync(Guid userId);
        Task<ApiResponse<CharityDto>> UpdateCharityAsync(Guid userId, UpdateCharityDto request);
        Task<ApiResponse<PagedResult<CharityDto>>> GetNearbyCharitiesAsync(double latitude, double longitude, double radiusKm, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<PagedResult<CharityDto>>> GetNearbyCharitiesForRestaurantAsync(Guid restaurantUserId, double radiusKm, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<IEnumerable<CharityDto>>> GetCharitiesByTypeAsync(CharityType type);
        Task<ApiResponse<bool>> UpdateStatusAsync(int charityId, ApprovalStatus status, string? rejectionReason = null);
        Task<ApiResponse<PagedResult<CharityDto>>> GetCharitiesForAdminAsync(int pageNumber, int pageSize, ApprovalStatus? status = null, string? searchTerm = null);
        Task<ApiResponse<bool>> DeleteMyCharityAsync(Guid userId);
        Task<ApiResponse<bool>> AdminDeleteCharityAsync(int charityId);
    }
}