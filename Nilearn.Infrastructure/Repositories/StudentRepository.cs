using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;


namespace Nilearn.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private AppDbContext _context;
        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Student student,CancellationToken cancellationToken = default)
        {
            await _context.Students.AddAsync(student,cancellationToken);
        }

        public async Task<Student?> GetByUserId(string userId, CancellationToken cancellationToken = default)
        {
            if (!Guid.TryParse(userId, out var parsedUserId))
                return null;

               
            var student = await _context.Students
                .AsNoTracking()
                .Include(s => s.AppUser)
                .FirstOrDefaultAsync(s => s.AppUserId == parsedUserId,cancellationToken);
            return student;
        }

        public void Update(Student student,CancellationToken cancellationToken = default)
        {
            _context.Students.Update(student);
        }
    }
}
