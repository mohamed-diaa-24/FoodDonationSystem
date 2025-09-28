using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Interfaces.IRepositories;

namespace FoodDonationSystem.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRestaurantRepository Restaurants { get; }
        ICharityRepository Charities { get; }
        IDonationRepository Donations { get; }
        IGenericRepository<DonationImage> DonationImages { get; }
        IReservationRepository Reservations { get; }
        IGenericRepository<RestaurantSchedule> RestaurantSchedules { get; }
        IGenericRepository<Volunteer> Volunteers { get; }
        IGenericRepository<VolunteerArea> VolunteerAreas { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<CharityNeed> CharityNeeds { get; }
        IGenericRepository<T> Repository<T>() where T : BaseEntity;
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
