using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Interfaces.IRepositories;
using FoodDonationSystem.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodDonationSystem.Data.Repositories
{
    public class RestaurantRepository : GenericRepository<Restaurant>, IRestaurantRepository
    {
        public RestaurantRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<Restaurant?> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId);
        }

        public async Task<IEnumerable<Restaurant>> GetApprovedRestaurantsAsync()
        {
            return await _dbSet
                .Include(r => r.User)
                .Where(r => r.Status == ApprovalStatus.Approved && r.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Restaurant>> GetNearbyRestaurantsAsync(double latitude, double longitude, double radiusKm)
        {
            // Using Haversine formula for distance calculation
            return await _dbSet
                .Include(r => r.User)
                .Where(r => r.Status == ApprovalStatus.Approved && r.IsActive)
                .Where(r =>
                    (6371 * Math.Acos(
                        Math.Cos(latitude * Math.PI / 180) *
                        Math.Cos(r.Latitude * Math.PI / 180) *
                        Math.Cos((r.Longitude - longitude) * Math.PI / 180) +
                        Math.Sin(latitude * Math.PI / 180) *
                        Math.Sin(r.Latitude * Math.PI / 180)
                    )) <= radiusKm)
                .ToListAsync();
        }

        public async Task<IEnumerable<Restaurant>> GetRestaurantsWithActiveDonationsAsync()
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Donations.Where(d => d.Status == DonationStatus.Available))
                .Where(r => r.Status == ApprovalStatus.Approved &&
                           r.IsActive &&
                           r.Donations.Any(d => d.Status == DonationStatus.Available))
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(int restaurantId, ApprovalStatus status, string? rejectionReason = null)
        {
            var restaurant = await GetByIdAsync(restaurantId);
            if (restaurant == null) return false;

            restaurant.Status = status;
            restaurant.RejectionReason = rejectionReason;
            await UpdateAsync(restaurant);
            return true;
        }

        public async Task<(IEnumerable<Restaurant> Restaurants, int TotalCount)> GetRestaurantsForAdminAsync(
            int pageNumber, int pageSize, ApprovalStatus? status = null, string? searchTerm = null)
        {
            var query = _dbSet.Include(r => r.User).AsQueryable();

            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(r =>
                    r.Name.Contains(searchTerm) ||
                    r.User.FirstName.Contains(searchTerm) ||
                    r.User.LastName.Contains(searchTerm) ||
                    r.User.Email.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var restaurants = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (restaurants, totalCount);
        }
    }
}
