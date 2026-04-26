
using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories;

internal class SectionRepository : ISectionRepository
{
    private readonly AppDbContext _context;
    public SectionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Section section, CancellationToken cancellationToken = default)
    {
        await _context.Sections.AddAsync(section, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        var section = await _context.Sections.FirstOrDefaultAsync(s => s.Id == sectionId && !s.IsDeleted, cancellationToken);
        if (section != null)
        {
            section.IsDeleted = true;
            section.UpdatedAt = DateTime.UtcNow;
            return true;
        }
        return false;

    }

    public async Task DecrementOrderFromAsync(int courseId, int fromOrder, CancellationToken cancellationToken = default)
    {
        await _context.Sections
            .Where(s => s.CourseId == courseId &&
                        s.Order >= fromOrder)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.Order, s => s.Order - 1),
                cancellationToken);
    }

    public async Task DecrementOrderRangeAsync(int courseId, int fromOrder, int toOrder, CancellationToken cancellationToken = default)
    {
        await _context.Sections
        .Where(s => s.CourseId == courseId &&
                    s.Order >= fromOrder &&
                    s.Order <= toOrder)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(s => s.Order, s => s.Order - 1),
            cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sections.AnyAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Section>> GetByCourseIdAsync(int courseId, CancellationToken cancellationToken = default)
    {

        var sections = await _context.Sections.AsNoTracking()
            .Where(s => s.CourseId == courseId && !s.IsDeleted)
            .OrderBy(s => s.Order)
            .ToListAsync(cancellationToken);
        return sections;
    }

    public async Task<Section?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sections.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }

    public async Task<int> GetMaxOrderAsync(int courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Sections

            .Where(s => s.CourseId == courseId && !s.IsDeleted)
            .MaxAsync(s => (int?)s.Order, cancellationToken) ?? 0;
    }
    public async Task IncrementOrderFromAsync(int courseId,int fromOrder,CancellationToken cancellationToken = default)
    {
        await _context.Sections
            .Where(s =>
                s.CourseId == courseId &&
                s.Order >= fromOrder)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.Order, s => s.Order + 1),
                cancellationToken);
    }
    public async Task IncrementOrderRangeAsync(int courseId, int fromOrder,int toOrder ,CancellationToken cancellationToken = default)
    {
        await _context.Sections
       .Where(s => s.CourseId == courseId &&
                   s.Order >= fromOrder &&
                   s.Order <= toOrder)
       .ExecuteUpdateAsync(setters => setters
           .SetProperty(s => s.Order, s => s.Order + 1),
           cancellationToken);
    }

    public void Update(Section section)
    {
        section.UpdatedAt = DateTime.UtcNow;
        _context.Sections.Update(section);
    }

    public void UpdateRange(IEnumerable<Section> sections)
    {
        foreach (var section in sections)
        {
            section.UpdatedAt = DateTime.UtcNow;
        }

        _context.Sections.UpdateRange(sections);
    }
    public async Task<bool> IsOwner(int sectionId, string userId, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(userId, out var parsedUserId))
            return false;

        return await _context.Sections
            .AnyAsync(s =>
                s.Id == sectionId &&
                !s.IsDeleted &&
                s.Course.Instructor.AppUserId == parsedUserId,
                cancellationToken);
    }
}
