using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces;

public interface IInstructorRepository
{
    Task AddAsync(Instructor instructor,CancellationToken cancellationToken = default);
    void Update(Instructor instructor);

    Task<int?> GetInstructorIdByUserIdAsync(string userId, CancellationToken cancellationToken = default);


}
