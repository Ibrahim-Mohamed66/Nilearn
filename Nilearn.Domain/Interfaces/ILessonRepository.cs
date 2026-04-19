using Nilearn.Domain.Entities;


namespace Nilearn.Domain.Interfaces;

public interface ILessonRepository
{
    Task AddAsync(Lesson lesson, CancellationToken cancellationToken = default);
    void Update(Lesson lesson);
    Task<bool> DeleteAsync(int lessonId, CancellationToken cancellationToken = default);
    Task<Lesson?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int lessonId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Lesson>> GetAllBySectionIdAsync(int sectionId, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderAsync(int sectionId, CancellationToken cancellationToken = default);
    
    Task IncrementOrderRangeAsync(int sectionId, int fromOrder, int toOrder, CancellationToken cancellationToken = default);
    Task IncrementOrderFromAsync(int sectionId, int fromOrder, CancellationToken cancellationToken = default);
    Task DecrementOrderFromAsync(int sectionId, int fromOrder, CancellationToken cancellationToken = default);
    Task DecrementOrderRangeAsync(int sectionId, int fromOrder, int toOrder, CancellationToken cancellationToken = default);
}
