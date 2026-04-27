using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment,CancellationToken cancellationToken = default);

    Task<Payment?> GetByIdAsync(int id,CancellationToken cancellationToken = default);

    Task<Payment?> GetByMerchantReferenceAsync(string merchantReference,CancellationToken cancellationToken = default);

    Task<bool> ExistsByMerchantReferenceAsync(string merchantReference,CancellationToken cancellationToken = default);

    Task<IEnumerable<Payment>> GetByEnrollmentIdAsync(int enrollmentId,CancellationToken cancellationToken = default);

    void Update(Payment payment);

}
