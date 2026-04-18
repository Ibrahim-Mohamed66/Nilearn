using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Nilearn.Domain.Entities;
using Nilearn.Shared.Models;

namespace Nilearn.Domain.Interfaces;

public interface ICourseRepository
{
    Task AddAsync(Course course, CancellationToken cancellationToken = default);
    void Update(Course course);
    Task<bool> DeleteAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    IQueryable<Course> GetAll();
    Task<bool> AnyAsync(int courseId, CancellationToken cancellationToken = default);
    Task<Course?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResponse<Course>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    IQueryable<Course> GetByInstructorId(int instructorId);
}
