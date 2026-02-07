using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyShopApp.Domain.Common;
using System.Data;

namespace MyShopApp.Infrastructure
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IDbContextTransaction _currentTransaction;
        public IDbContextTransaction CurrentTransaction => _currentTransaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if(_currentTransaction ==  null) 
            {
                throw new Exception("Текущая транзакция отсутствует (null).");
            }

            try
            {
                await SaveChangesAsync(ct);
                await _currentTransaction.CommitAsync(ct);
            }
            catch (Exception)
            {

                await RollbackTransactionAsync(ct);
                throw;
            }
            finally
            {
                DisposeCurrentTransaction();
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            try
            {
                await _currentTransaction?.RollbackAsync(ct);
            }
            finally
            {
                DisposeCurrentTransaction();
            }
        }

        private void DisposeCurrentTransaction()
        {
            if( _currentTransaction != null ) 
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
