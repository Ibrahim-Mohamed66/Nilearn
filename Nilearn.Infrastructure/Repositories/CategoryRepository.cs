using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories;

internal class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
    }

    public async Task DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId && !c.IsDeleted, cancellationToken);

        if (category != null)
        {
            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;
        }
    }

    public IQueryable<Category> GetAllCategories()
    {
        return _context.Categories
            .Where(c => !c.IsDeleted)
            .AsNoTracking();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public void UpdateCategory(Category category)
    {
        _context.Categories.Update(category);
    }
}