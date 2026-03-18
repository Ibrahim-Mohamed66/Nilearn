using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;


namespace Nilearn.Infrastructure.Repositories
{
    internal class InstructorRepositroy : IInstructorRepository
    {
        private readonly AppDbContext _context;
        public InstructorRepositroy(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Instructor instructor, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(instructor, cancellationToken);
        }

        public void Update(Instructor instructor)
        {
            _context.Update(instructor);
        }
    }
}
