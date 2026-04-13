using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Nilearn.Domain.Entities;
using Nilearn.Shared.Models;

namespace Nilearn.Domain.Interfaces;

public interface ICourseRepository
{
    Task AddCourseAsync(Course course, CancellationToken cancellationToken = default);
    void UpdateCourse(Course course);
    Task DeleteCourseAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Course?> GetCourseByIdAsync(int id, CancellationToken cancellationToken = default);
    IQueryable<Course> GetAllCourses();
    Task<Course?> GetCourseByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResponse<Course>> GetPagedCoursesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    IQueryable<Course> GetCoursesByInstructorId(int instructorId);
}
