using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Nilearn.Application.Common.Extensions;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;
using Nilearn.Shared.Models;

namespace Nilearn.Infrastructure.Repositories;

internal class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;
    public CourseRepository(AppDbContext context)
    {
       _context = context;
    }

    public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        await _context.Courses.AddAsync(course, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int courseId, CancellationToken cancellationToken = default)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId && !c.IsDeleted, cancellationToken);
        if (course != null)
        {
            course.IsDeleted = true;
            course.UpdatedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }

    public IQueryable<Course> GetAll()
    {
        return _context.Courses.AsNoTracking().Where(c => !c.IsDeleted);
    }

    public async Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }
    public IQueryable<Course> GetByInstructorId(int instructorId)
    {
        return _context.Courses
            .AsNoTracking()
            .Where(c => c.InstructorId == instructorId && !c.IsDeleted)
            .Include(c => c.Category)
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User);
    }
    
    public async Task<Course?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public void Update(Course course)
    {
        course.UpdatedAt = DateTime.UtcNow;
        _context.Courses.Update(course);
    }

    public async Task<PagedResponse<Course>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Courses
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Include(c => c.Category)
            .Include(c => c.Instructor)
                .ThenInclude(i => i.User);

        return await query.ToPagedAsync(pageNumber, pageSize, cancellationToken);
    }

    public Task<bool> AnyAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return _context.Courses.AnyAsync(c => c.Id == courseId && !c.IsDeleted, cancellationToken);
    }
}
