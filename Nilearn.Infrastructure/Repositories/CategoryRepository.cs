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

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId && !c.IsDeleted, cancellationToken);

        if (category != null)
        {
            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }

    public IQueryable<Category> GetAll()
    {
        return _context.Categories
            .Where(c => !c.IsDeleted)
            .AsNoTracking();
    }
    


    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    public Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
       return _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name && !c.IsDeleted, cancellationToken);
    }

    public void Update(Category category)
    {
        category.UpdatedAt = DateTime.UtcNow;
        _context.Categories.Update(category);
    }
}