using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;
using Nilearn.Infrastructure.Persistence.Extensions;
using Nilearn.Shared.Models;

namespace Nilearn.Infrastructure.Repositories;

internal class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;
    public CourseRepository(AppDbContext context)
    {
       _context = context;
    }

    public async Task AddCourseAsync(Course course, CancellationToken cancellationToken = default)
    {
        await _context.Courses.AddAsync(course, cancellationToken);
    }

    public async Task DeleteCourseAsync(int courseId, CancellationToken cancellationToken = default)
    {
        var course = await _context.Courses.FindAsync(courseId, cancellationToken);
        if (course != null)
        {
            course.IsDeleted = true;
            course.UpdatedAt = DateTime.UtcNow;
        }
    }

    public IQueryable<Course> GetAllCourses()
    {
        return _context.Courses.Where(c => !c.IsDeleted);
    }

    public async Task<Course?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Courses.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public async Task<PagedResponse<Course>> GetPagedCoursesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Courses
             .AsNoTracking()
             .Where(c => !c.IsDeleted)
             .OrderBy(c => c.Title)
             .ToPagedAsync(pageNumber, pageSize, cancellationToken);
    }

    public void UpdateCourse(Course course)
    {
        _context.Courses.Update(course);
    }
}
