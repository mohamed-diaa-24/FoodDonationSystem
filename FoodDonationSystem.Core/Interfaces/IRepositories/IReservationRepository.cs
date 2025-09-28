using FoodDonationSystem.Core.Entities;

namespace FoodDonationSystem.Core.Interfaces.IRepositories
{
    public interface IReservationRepository : IGenericRepository<Reservation>
    {
        Task<(IEnumerable<Reservation> Items, int TotalCount)> GetPagedWithDetailsAsync(
            int pageNumber,
            int pageSize,
            System.Linq.Expressions.Expression<Func<Reservation, bool>>? predicate = null,
            System.Linq.Expressions.Expression<Func<Reservation, object>>? orderBy = null,
            bool orderByDescending = false);
    }
}
