using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces;

public interface IInstructorWalletRepository
{
    Task<InstructorWallet?> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken);
    Task AddAsync(InstructorWallet wallet, CancellationToken cancellationToken);
}
