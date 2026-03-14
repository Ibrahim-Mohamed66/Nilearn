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

        public void Update(Student student,CancellationToken cancellationToken = default)
        {
            _context.Students.Update(student);
        }
    }
}
