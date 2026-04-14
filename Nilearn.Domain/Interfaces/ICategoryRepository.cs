using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddAsync(Category category,CancellationToken cancellationToken = default);
        void Update(Category category);
        Task<bool> DeleteAsync(int categoryId,CancellationToken cancellationToken = default);
        Task<Category?> GetByIdAsync(int id,CancellationToken cancellationToken = default);
        Task<Category?> GetByNameAsync(string name,CancellationToken cancellationToken = default);
        IQueryable<Category> GetAll();
       

    }
}
