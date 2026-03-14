using Microsoft.EntityFrameworkCore.Storage;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private IRefreshTokenRepository? _refreshTokenRepository;
        private IStudentRepository? _studentRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IRefreshTokenRepository RefreshTokenRepository
        {
            get
            {
                return _refreshTokenRepository ??= new RefreshTokenRepository(_context);
            }
        }
        public IStudentRepository StudentRepository
        {
            get
            {
                return _studentRepository ??= new StudentRepository(_context);
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No transaction in progress.");

            try
            {
                await _transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await DisposeTransactionAsync();
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null) return;

            await _transaction.RollbackAsync(cancellationToken);
            await DisposeTransactionAsync();
        }

        private async Task DisposeTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeTransactionAsync();
            await _context.DisposeAsync();
        }
    }
}