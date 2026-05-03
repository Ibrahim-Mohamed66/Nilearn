using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories
{
    internal class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;
        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
        {
            await _context.Enrollments.AddAsync(enrollment, cancellationToken);
        }

        public async Task<Enrollment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }
        public async Task<Enrollment?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                    .ThenInclude(s => s.AppUser)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                        .ThenInclude(i => i.User)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<Enrollment?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default)
        {
            return await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId)
                .ToListAsync(cancellationToken);
        }

        public IQueryable<Enrollment> QueryByCourseId(int courseId, EnrollmentStatus? status = null)
        {
            var query = _context.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseId == courseId && !e.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            return query.Include(e => e.Student);
        }

        public IQueryable<Enrollment> QueryByStudentId(int studentId, EnrollmentStatus? status = null)
        {
            var query = _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId);

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            return query.Include(e => e.Course);
              

        }



        public async Task<bool> IsEnrolledAsync(int studentId, int courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.Status == EnrollmentStatus.Active, cancellationToken);
        }

        public void Update(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var enrollment = await _context.Enrollments.FindAsync(id, cancellationToken);
            if (enrollment is null)
                return false;
            enrollment.IsDeleted = true;
            return true;
        }
    }
}
