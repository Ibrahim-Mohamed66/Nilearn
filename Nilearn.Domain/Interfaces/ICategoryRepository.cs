using Nilearn.Domain.Entities;
using Nilearn.Shared.Models;

namespace Nilearn.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddCategoryAsync(Category category,CancellationToken cancellationToken = default);
        void UpdateCategory(Category category);
        Task DeleteCategoryAsync(int categoryId,CancellationToken cancellationToken = default);
        Task<Category?> GetCategoryByIdAsync(int id,CancellationToken cancellationToken = default);
        Task<Category?> GetCategoryByNameAsync(string name,CancellationToken cancellationToken = default);
        IQueryable<Category> GetAllCategories();
        Task<PagedResponse<Category>> GetPagedCategoriesAsync(int pageNumber, int pageSize,CancellationToken cancellationToken=default);

    }
}
