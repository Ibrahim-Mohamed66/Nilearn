namespace Nilearn.Domain.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IStudentRepository StudentRepository { get; }
        public IInstructorRepository InstructorRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public ISectionRepository SectionRepository { get; }
        public ILessonRepository LessonRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    }
}
