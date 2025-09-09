using FoodDonationSystem.Core.Entities;
using FoodDonationSystem.Core.Interfaces;
using FoodDonationSystem.Core.Interfaces.IRepositories;
using FoodDonationSystem.Data.Context;
using FoodDonationSystem.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FoodDonationSystem.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;


        private IRestaurantRepository? _restaurants;
        private ICharityRepository? _charities;
        private readonly Dictionary<Type, object> _repositories = new();


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }


        public IRestaurantRepository Restaurants => _restaurants ??= new RestaurantRepository(_context);
        public ICharityRepository Charities =>
            _charities ??= new CharityRepository(_context);

        public IGenericRepository<Donation> Donations => Repository<Donation>();
        public IGenericRepository<DonationImage> DonationImages => Repository<DonationImage>();
        public IGenericRepository<Reservation> Reservations => Repository<Reservation>();
        public IGenericRepository<RestaurantSchedule> RestaurantSchedules => Repository<RestaurantSchedule>();
        public IGenericRepository<Volunteer> Volunteers => Repository<Volunteer>();
        public IGenericRepository<VolunteerArea> VolunteerAreas => Repository<VolunteerArea>();
        public IGenericRepository<Review> Reviews => Repository<Review>();
        public IGenericRepository<CharityNeed> CharityNeeds => Repository<CharityNeed>();

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            if (_repositories.ContainsKey(type))
            {
                return (IGenericRepository<T>)_repositories[type];
            }

            var repositoryInstance = new GenericRepository<T>(_context);
            _repositories.Add(type, repositoryInstance);
            return repositoryInstance;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
