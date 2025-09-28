using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Interfaces.IRepositories;
using FoodDonationSystem.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FoodDonationSystem.Data.Repositories
{
    public class ReservationRepository : GenericRepository<Reservation>, IReservationRepository
    {
        public ReservationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Reservation> Items, int TotalCount)> GetPagedWithDetailsAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<Reservation, bool>>? predicate = null,
            Expression<Func<Reservation, object>>? orderBy = null,
            bool orderByDescending = false)
        {
            var query = _dbSet
                .Include(r => r.Donation)
                    .ThenInclude(d => d.Restaurant)
                        .ThenInclude(rest => rest.User)
                .AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);

            var totalCount = await query.CountAsync();

            if (orderBy != null)
            {
                query = orderByDescending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);
            }

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
