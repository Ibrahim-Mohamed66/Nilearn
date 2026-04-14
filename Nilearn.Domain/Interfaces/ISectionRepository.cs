using Nilearn.Domain.Entities;


namespace Nilearn.Domain.Interfaces
{
    public interface ISectionRepository
    {
        Task AddAsync(Section section, CancellationToken cancellationToken = default);
        void Update(Section section);
        Task<bool> DeleteAsync(int sectionId, CancellationToken cancellationToken = default);
        Task<Section?> GetByIdAsync(int id,CancellationToken cancellationToken = default);
        Task<int> GetMaxOrderAsync(int courseId,CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id,CancellationToken cancellationToken =default);
        Task<IEnumerable<Section>> GetByCourseIdAsync(int courseId,CancellationToken cancellationToken = default);
        void UpdateRange(IEnumerable<Section> sections);


    }
}
