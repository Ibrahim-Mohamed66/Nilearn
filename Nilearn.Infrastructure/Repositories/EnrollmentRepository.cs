using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
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

        public async Task<bool> IsEnrolledAsync(int studentId, int courseId, CancellationToken cancellationToken = default)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);
        }

        public void Update(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
        }
    }
}
