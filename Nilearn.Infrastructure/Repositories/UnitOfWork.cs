using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Nilearn.Domain.Entities;
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
        private IInstructorRepository? _instructorRepository;
        private ICategoryRepository? _categoryRepository;
        private ICourseRepository? _courseRepository;
        private ISectionRepository? _sectionRepository;
        private ILessonRepository? _lessonRepository;
        private IEnrollmentRepository? _enrollmentRepository;
        private IPaymentRepository? _paymentRepository;
        private IWalletTransactionRepository? _walletTransactionRepository;
        private IPlatformWalletRepository? _platformWalletRepository;
        private IInstructorWalletRepository? _instructorWalletRepository;

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
        public ISectionRepository SectionRepository
        {
            get
            {
                return _sectionRepository ??= new SectionRepository(_context);
            }
        }

        public IInstructorRepository InstructorRepository
        {
            get
            {
                return _instructorRepository ??= new InstructorRepositroy(_context);
            }
        }
        public ICategoryRepository CategoryRepository
        {
            get
            {
                return _categoryRepository ??= new CategoryRepository(_context);
            }
        }

        public ICourseRepository CourseRepository
        {
            get
            {
                return _courseRepository ??= new CourseRepository(_context);
            }
        }
        public ILessonRepository LessonRepository
        {
            get
            {
                return _lessonRepository ??= new LessonRepository(_context);
            }
        }

        public IEnrollmentRepository EnrollmentRepository
        {
            get
            {
                return _enrollmentRepository ??= new EnrollmentRepository(_context);
            }
        }

        public IPaymentRepository PaymentRepository
        {
            get
            {
                return _paymentRepository ??= new PaymentRepository(_context);
            }
        }
        public IWalletTransactionRepository WalletTransactionRepository
        {
            get
            {
                return _walletTransactionRepository ??= new WalletTransactionRepository(_context);
            }
        }
        public IPlatformWalletRepository PlatformWalletRepository
        {
            get
            {
                return _platformWalletRepository ??= new PlatformWalletRepository(_context);
            }
        }
        public IInstructorWalletRepository InstructorWalletRepository
        {
            get
            {
                return _instructorWalletRepository ??= new InstructorWalletRepository(_context);
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