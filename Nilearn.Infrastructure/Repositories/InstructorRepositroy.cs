using Microsoft.EntityFrameworkCore;
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
            await _context.Instructors.AddAsync(instructor, cancellationToken);
        }

        public void Update(Instructor instructor)
        {
            _context.Instructors.Update(instructor);
        }

        public async Task<int?> GetIdByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Instructors
                .Where(i => i.AppUserId.ToString() == userId)
                .Select(i => i.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
