using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Enums;

namespace FoodDonationSystem.Core.Interfaces.IRepositories
{
    public interface ICharityRepository : IGenericRepository<Charity>
    {
        Task<Charity?> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Charity>> GetApprovedCharitiesAsync();
        Task<IEnumerable<Charity>> GetNearbyCharitiesAsync(double latitude, double longitude, double radiusKm);
        Task<IEnumerable<Charity>> GetCharitiesByTypeAsync(CharityType type);
        Task<bool> UpdateStatusAsync(int charityId, ApprovalStatus status, string? rejectionReason = null);
        Task<(IEnumerable<Charity> Charities, int TotalCount)> GetCharitiesForAdminAsync(
            int pageNumber, int pageSize, ApprovalStatus? status = null, CharityType? type = null);
    }
}
