using Nilearn.Domain.Entities;


namespace Nilearn.Domain.Interfaces;

public interface IEnrollmentRepository
{
    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    Task<bool> IsEnrolledAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    void Update(Enrollment enrollment);
}
