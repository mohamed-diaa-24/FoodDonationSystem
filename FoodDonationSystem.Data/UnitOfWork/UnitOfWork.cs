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



        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }


        public IRestaurantRepository Restaurants => _restaurants ??= new RestaurantRepository(_context);


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
