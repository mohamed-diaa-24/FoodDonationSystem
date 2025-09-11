using FoodDonationSystem.Core.DTOs.Donation;
using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Interfaces.IRepositories;
using FoodDonationSystem.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodDonationSystem.Data.Repositories
{
    public class DonationRepository : GenericRepository<Donation>, IDonationRepository
    {
        public DonationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Donation?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Donations
                .Include(d => d.Restaurant)
                .Include(d => d.Images)
                .Include(d => d.Reservations)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<Donation?> GetByIdWithImagesAsync(int id)
        {
            return await _context.Donations
                .Include(d => d.Images)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<IEnumerable<Donation>> GetByRestaurantIdAsync(int restaurantId)
        {
            return await _context.Donations
                .Include(d => d.Images)
                .Where(d => d.RestaurantId == restaurantId && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Donations
                .Include(d => d.Restaurant)
                .Include(d => d.Images)
                .Where(d => d.Restaurant.UserId == userId && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetAvailableDonationsAsync()
        {
            return await _context.Donations
                .Include(d => d.Restaurant)
                .Include(d => d.Images)
                .Where(d => d.Status == DonationStatus.Available &&
                           d.ExpiryDateTime > DateTime.UtcNow &&
                           !d.IsDeleted)
                .OrderBy(d => d.ExpiryDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetExpiredDonationsAsync()
        {
            return await _context.Donations
                .Include(d => d.Restaurant)
                .Where(d => d.ExpiryDateTime <= DateTime.UtcNow && !d.IsDeleted)
                .OrderByDescending(d => d.ExpiryDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetDonationsByStatusAsync(DonationStatus status)
        {
            return await _context.Donations
                .Include(d => d.Restaurant)
                .Include(d => d.Images)
                .Where(d => d.Status == status && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetNearbyDonationsAsync(double latitude, double longitude, double radiusKm)
        {
            return await _context.Donations
                   .Where(d => !d.IsDeleted && d.Status == DonationStatus.Available && d.ExpiryDateTime > DateTime.UtcNow)
                   .Join(
                       _context.Restaurants.Where(r => !r.IsDeleted),
                       d => d.RestaurantId,
                       r => r.Id,
                       (d, r) => new { Donation = d, Restaurant = r }
                   )
                   .Where(x =>
                       6371 * Math.Acos(
                           Math.Cos(latitude * Math.PI / 180) *
                           Math.Cos(x.Restaurant.Latitude * Math.PI / 180) *
                           Math.Cos((x.Restaurant.Longitude - longitude) * Math.PI / 180) +
                           Math.Sin(latitude * Math.PI / 180) *
                           Math.Sin(x.Restaurant.Latitude * Math.PI / 180)
                       ) <= radiusKm
                   )
                   .Select(x => x.Donation)
                   .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetDonationsByFoodTypeAsync(string foodType)
        {
            return await _context.Donations
                .Include(d => d.Restaurant)
                .Include(d => d.Images)
                .Where(d => d.FoodType.Contains(foodType) && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetDonationsRequiringPickupAsync()
        {
            return await _context.Donations
                .Include(d => d.Restaurant)
                .Include(d => d.Images)
                .Where(d => d.RequiresPickup &&
                           d.Status == DonationStatus.Available &&
                           d.ExpiryDateTime > DateTime.UtcNow &&
                           !d.IsDeleted)
                .OrderBy(d => d.ExpiryDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetDonationsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Donations
                .Include(d => d.Restaurant)
                .Include(d => d.Images)
                .Where(d => d.CreatedAt >= fromDate &&
                           d.CreatedAt <= toDate &&
                           !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Donation> Items, int TotalCount)> GetDonationsForAdminAsync(
            int pageNumber, int pageSize, DonationStatus? status = null, string? searchTerm = null)
        {
            var query = _context.Donations
                .Include(d => d.Restaurant)
                .Include(d => d.Images)
                .Where(d => !d.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(d => d.FoodType.Contains(searchTerm) ||
                                       d.Description.Contains(searchTerm) ||
                                       d.Restaurant.Name.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<Donation> Items, int TotalCount)> GetDonationsWithFilterAsync(
            int pageNumber, int pageSize, DonationFilterDto filter)
        {
            var query = _context.Donations
                .Include(d => d.Restaurant)
                .Include(d => d.Images)
                .Where(d => !d.IsDeleted);

            if (filter.Status.HasValue)
            {
                query = query.Where(d => d.Status == filter.Status.Value);
            }

            if (!string.IsNullOrEmpty(filter.FoodType))
            {
                query = query.Where(d => d.FoodType.Contains(filter.FoodType));
            }

            if (filter.Latitude.HasValue && filter.Longitude.HasValue && filter.RadiusKm.HasValue)
            {
                query = query.Where(d => CalculateDistance(
                    filter.Latitude.Value, filter.Longitude.Value,
                    d.Restaurant.Latitude, d.Restaurant.Longitude) <= filter.RadiusKm.Value);
            }

            if (filter.RequiresPickup.HasValue)
            {
                query = query.Where(d => d.RequiresPickup == filter.RequiresPickup.Value);
            }

            if (filter.IsExpired.HasValue)
            {
                if (filter.IsExpired.Value)
                {
                    query = query.Where(d => d.ExpiryDateTime <= DateTime.UtcNow);
                }
                else
                {
                    query = query.Where(d => d.ExpiryDateTime > DateTime.UtcNow);
                }
            }

            if (filter.ExpiryDateFrom.HasValue)
            {
                query = query.Where(d => d.ExpiryDateTime >= filter.ExpiryDateFrom.Value);
            }

            if (filter.ExpiryDateTo.HasValue)
            {
                query = query.Where(d => d.ExpiryDateTime <= filter.ExpiryDateTo.Value);
            }

            if (filter.MinServings.HasValue)
            {
                query = query.Where(d => d.EstimatedServings >= filter.MinServings.Value);
            }

            if (filter.MaxServings.HasValue)
            {
                query = query.Where(d => d.EstimatedServings <= filter.MaxServings.Value);
            }

            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(d => d.FoodType.Contains(filter.SearchTerm) ||
                                       d.Description.Contains(filter.SearchTerm) ||
                                       d.Restaurant.Name.Contains(filter.SearchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(d => d.ExpiryDateTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> UpdateStatusAsync(int donationId, DonationStatus status)
        {
            var donation = await _context.Donations.FindAsync(donationId);
            if (donation == null || donation.IsDeleted)
                return false;

            donation.Status = status;
            donation.UpdatedAt = DateTime.UtcNow;
            return true;
        }

        public async Task<int> GetDonationCountByRestaurantAsync(int restaurantId)
        {
            return await _context.Donations
                .Where(d => d.RestaurantId == restaurantId && !d.IsDeleted)
                .CountAsync();
        }

        public async Task<int> GetAvailableDonationCountAsync()
        {
            return await _context.Donations
                .Where(d => d.Status == DonationStatus.Available &&
                           d.ExpiryDateTime > DateTime.UtcNow &&
                           !d.IsDeleted)
                .CountAsync();
        }

        public async Task<IEnumerable<Donation>> GetDonationsByExpiryDateAsync(DateTime expiryDate)
        {
            return await _context.Donations
                .Include(d => d.Restaurant)
                .Where(d => d.ExpiryDateTime.Date == expiryDate.Date && !d.IsDeleted)
                .OrderBy(d => d.ExpiryDateTime)
                .ToListAsync();
        }

        public async Task<bool> HasActiveReservationsAsync(int donationId)
        {
            return await _context.Reservations
                .AnyAsync(r => r.DonationId == donationId &&
                              r.Status == ReservationStatus.Confirmed &&
                              !r.IsDeleted);
        }

        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadius = 6371; // Earth's radius in kilometers

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadius * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}

