using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Review?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    IQueryable<Review> QueryByCourseId(int courseId);
    Task<int> GetTotalCountByCourseIdAsync(int courseId, CancellationToken cancellationToken = default);
    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    void Update(Review review);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
