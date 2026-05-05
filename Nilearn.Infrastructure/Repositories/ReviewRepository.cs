using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories;

internal class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Review?> GetByStudentAndCourseAsync(int studentId, int courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.StudentId == studentId && r.CourseId == courseId, cancellationToken);
    }

    public IQueryable<Review> QueryByCourseId(int courseId)
    {
        return _context.Reviews
            .AsNoTracking()
            .Where(r => r.CourseId == courseId)
            .Include(r => r.Student)
                .ThenInclude(s => s.AppUser)
            .OrderByDescending(r => r.CreatedAt);
    }

    public async Task<int> GetTotalCountByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .CountAsync(r => r.CourseId == courseId, cancellationToken);
    }

    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _context.Reviews.AddAsync(review, cancellationToken);
    }

    public void Update(Review review)
    {
        _context.Reviews.Update(review);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var review = await _context.Reviews.FindAsync(id, cancellationToken);
        if (review is null)
            return false;
        review.IsDeleted = true;
        return true;
    }
}
