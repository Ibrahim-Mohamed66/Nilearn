using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces;

public interface IWalletTransactionRepository
{
    Task AddAsync(WalletTransaction transaction, CancellationToken cancellationToken);
    Task<WalletTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> ExistsByPaymentAndInstructorAsync(int paymentId, int? instructorId, CancellationToken cancellationToken);

    Task<List<WalletTransaction>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken);

    Task<List<WalletTransaction>> GetByPaymentIdAsync(int paymentId, CancellationToken cancellationToken);
}
