using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Interfaces.IRepositories
{
    public interface IRestaurantRepository : IGenericRepository<Restaurant>
    {
        Task<Restaurant?> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Restaurant>> GetApprovedRestaurantsAsync();
        Task<IEnumerable<Restaurant>> GetNearbyRestaurantsAsync(double latitude, double longitude, double radiusKm);
        Task<IEnumerable<Restaurant>> GetRestaurantsWithActiveDonationsAsync();
        Task<bool> UpdateStatusAsync(int restaurantId, ApprovalStatus status, string? rejectionReason = null);
        Task<(IEnumerable<Restaurant> Restaurants, int TotalCount)> GetRestaurantsForAdminAsync(
            int pageNumber, int pageSize, ApprovalStatus? status = null, string? searchTerm = null);
    }
}
