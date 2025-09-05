using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Enums;
using FoodDonationSystem.Core.Interfaces.IRepositories;
using FoodDonationSystem.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodDonationSystem.Data.Repositories
{
    public class CharityRepository : GenericRepository<Charity>, ICharityRepository
    {
        public CharityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Charity?> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Needs)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<IEnumerable<Charity>> GetApprovedCharitiesAsync()
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.Status == ApprovalStatus.Approved && c.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Charity>> GetNearbyCharitiesAsync(double latitude, double longitude, double radiusKm)
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.Status == ApprovalStatus.Approved && c.IsActive)
                .Where(c =>
                    (6371 * Math.Acos(
                        Math.Cos(latitude * Math.PI / 180) *
                        Math.Cos(c.Latitude * Math.PI / 180) *
                        Math.Cos((c.Longitude - longitude) * Math.PI / 180) +
                        Math.Sin(latitude * Math.PI / 180) *
                        Math.Sin(c.Latitude * Math.PI / 180)
                    )) <= radiusKm)
                .ToListAsync();
        }

        public async Task<IEnumerable<Charity>> GetCharitiesByTypeAsync(CharityType type)
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.Type == type && c.Status == ApprovalStatus.Approved && c.IsActive)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(int charityId, ApprovalStatus status, string? rejectionReason = null)
        {
            var charity = await GetByIdAsync(charityId);
            if (charity == null) return false;

            charity.Status = status;
            charity.RejectionReason = rejectionReason;

            await UpdateAsync(charity);
            return true;
        }

        public async Task<(IEnumerable<Charity> Charities, int TotalCount)> GetCharitiesForAdminAsync(
            int pageNumber, int pageSize, ApprovalStatus? status = null, string? searchTerm = null)
        {
            var query = _dbSet.Include(c => c.User).AsQueryable();

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(searchTerm) ||
                    c.User.FirstName.Contains(searchTerm) ||
                    c.User.LastName.Contains(searchTerm) ||
                    c.User.Email.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var charities = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (charities, totalCount);
        }
    }
}
