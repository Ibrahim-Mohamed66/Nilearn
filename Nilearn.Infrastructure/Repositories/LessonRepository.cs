using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories;

internal class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;
    public LessonRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Lesson lesson, CancellationToken cancellationToken = default)
    {
        await _context.Lessons.AddAsync(lesson, cancellationToken);
    }

    public async Task<bool> ExistsAsync(int lessonId, CancellationToken cancellationToken = default)
    {
        return await _context.Lessons.AnyAsync(l => l.Id == lessonId && !l.IsDeleted, cancellationToken);
    }

    public async Task DecrementOrderFromAsync(int sectionId, int fromOrder, CancellationToken cancellationToken = default)
    {
        await _context.Lessons
           .Where(l => l.SectionId == sectionId && !l.IsDeleted &&
                       l.Order >= fromOrder)
           .ExecuteUpdateAsync(setters => setters
               .SetProperty(l => l.Order, l => l.Order - 1),
               cancellationToken);
    }

    public async Task DecrementOrderRangeAsync(int sectionId, int fromOrder, int toOrder, CancellationToken cancellationToken = default)
    {
        await _context.Lessons
           .Where(l => l.SectionId == sectionId && !l.IsDeleted &&
                       l.Order >= fromOrder &&
                       l.Order <= toOrder)
           .ExecuteUpdateAsync(setters => setters
               .SetProperty(l => l.Order, l => l.Order - 1),
               cancellationToken);
    }

    public async Task<bool> DeleteAsync(int lessonId, CancellationToken cancellationToken = default)
    {
        var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId && !l.IsDeleted,cancellationToken);
        if (lesson != null)
        {
            lesson.IsDeleted = true;
            lesson.UpdatedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }

   
    public async Task<Lesson?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Lessons.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Lesson>> GetAllBySectionIdAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        return await _context.Lessons.AsNoTracking()
            .Where(l => l.SectionId == sectionId && !l.IsDeleted)
            .OrderBy(l => l.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task IncrementOrderFromAsync(int sectionId, int fromOrder, CancellationToken cancellationToken = default)
    {
        await _context.Lessons
            .Where(l => l.SectionId == sectionId && !l.IsDeleted &&
                        l.Order >= fromOrder)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(l => l.Order, l => l.Order + 1),
                cancellationToken);
    }

    public async Task IncrementOrderRangeAsync(int sectionId, int fromOrder, int toOrder, CancellationToken cancellationToken = default)
    {
        await _context.Lessons
            .Where(l => l.SectionId == sectionId && !l.IsDeleted &&
                        l.Order >= fromOrder &&
                        l.Order <= toOrder)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(l => l.Order, l => l.Order + 1),
                cancellationToken);
    }

    public void Update(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
    }

    public async Task<int> GetMaxOrderAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        return await _context.Lessons
            .Where(l => l.SectionId == sectionId && !l.IsDeleted)
            .MaxAsync(l => (int?)l.Order, cancellationToken) ?? 0;
    }
    public async Task<Lesson?> GetNextLessonAsync(int sectionId, int currentOrder, CancellationToken cancellationToken = default)
    {
        return await _context.Lessons
            .AsNoTracking()
            .Where(l => l.SectionId == sectionId &&
                        !l.IsDeleted &&
                        l.Order > currentOrder)
            .OrderBy(l => l.Order)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Lesson?> GetPreviousLessonAsync(int sectionId, int currentOrder, CancellationToken cancellationToken = default)
    {
        return await _context.Lessons
            .AsNoTracking()
            .Where(l => l.SectionId == sectionId &&
                        !l.IsDeleted &&
                        l.Order < currentOrder)
            .OrderByDescending(l => l.Order)
            .FirstOrDefaultAsync(cancellationToken);
    }


}
