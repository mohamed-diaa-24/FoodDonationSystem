using FoodDonationSystem.Core.DTOs.Donation;
using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Interfaces.IRepositories
{
    public interface IDonationRepository : IGenericRepository<Donation>
    {
        Task<Donation?> GetByIdWithDetailsAsync(int id);
        Task<Donation?> GetByIdWithImagesAsync(int id);
        Task<IEnumerable<Donation>> GetByRestaurantIdAsync(int restaurantId);
        Task<IEnumerable<Donation>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Donation>> GetAvailableDonationsAsync();
        Task<IEnumerable<Donation>> GetExpiredDonationsAsync();
        Task<IEnumerable<Donation>> GetDonationsByStatusAsync(DonationStatus status);
        Task<IEnumerable<Donation>> GetNearbyDonationsAsync(double latitude, double longitude, double radiusKm);
        Task<IEnumerable<Donation>> GetDonationsByFoodTypeAsync(string foodType);
        Task<IEnumerable<Donation>> GetDonationsRequiringPickupAsync();
        Task<IEnumerable<Donation>> GetDonationsByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<(IEnumerable<Donation> Items, int TotalCount)> GetDonationsForAdminAsync(
            int pageNumber, int pageSize, DonationStatus? status = null, string? searchTerm = null);
        Task<(IEnumerable<Donation> Items, int TotalCount)> GetDonationsWithFilterAsync(
            int pageNumber, int pageSize, DonationFilterDto filter);
        Task<bool> UpdateStatusAsync(int donationId, DonationStatus status);
        Task<int> GetDonationCountByRestaurantAsync(int restaurantId);
        Task<int> GetAvailableDonationCountAsync();
        Task<IEnumerable<Donation>> GetDonationsByExpiryDateAsync(DateTime expiryDate);
        Task<bool> HasActiveReservationsAsync(int donationId);
    }
}

