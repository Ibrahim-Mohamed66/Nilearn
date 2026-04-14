
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
}
