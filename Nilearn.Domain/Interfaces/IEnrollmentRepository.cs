using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Enums;

namespace Nilearn.Domain.Interfaces;

public interface IEnrollmentRepository
{
    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    Task<bool> IsEnrolledAsync(int studentId, int courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    IQueryable<Enrollment> QueryByCourseId(int courseId, EnrollmentStatus? status = null);
    IQueryable<Enrollment> QueryByStudentId(int studentId, EnrollmentStatus? status = null);
    void Update(Enrollment enrollment);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
