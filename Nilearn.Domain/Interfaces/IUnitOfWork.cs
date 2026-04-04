namespace Nilearn.Domain.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IStudentRepository StudentRepository { get; }
        public IInstructorRepository InstructorRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    }
}
