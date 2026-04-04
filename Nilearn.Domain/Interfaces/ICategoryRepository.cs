using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddCategoryAsync(Category category,CancellationToken cancellationToken = default);
        void UpdateCategory(Category category);
        Task DeleteCategoryAsync(int categoryId,CancellationToken cancellationToken = default);
        Task<Category?> GetCategoryByIdAsync(int id,CancellationToken cancellationToken = default);
        IQueryable<Category> GetAllCategories();

    }
}
