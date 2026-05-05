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
    Task<PagedResponse<Course>> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null, 
        string? categoryName = null, 
        string? instructorName = null, 
        CancellationToken cancellationToken = default);
    IQueryable<Course> GetByInstructorId(int instructorId);
    Task<bool> IsOwner(int courseId, string userId, CancellationToken cancellationToken = default);
    
}
