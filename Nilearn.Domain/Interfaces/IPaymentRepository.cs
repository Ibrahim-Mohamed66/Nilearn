using Nilearn.Domain.Entities;
using Nilearn.Domain.Enums;

namespace Nilearn.Domain.Interfaces;

public interface IPaymentRepository
{
    IQueryable<Payment> QueryPaymentHistory(int studentId,PaymentStatus? status = null);

    Task AddAsync(Payment payment,CancellationToken cancellationToken = default);

    Task<Payment?> GetByIdAsync(int id,CancellationToken cancellationToken = default);

    Task<Payment?> GetByMerchantReferenceAsync(string merchantReference,CancellationToken cancellationToken = default);

    Task<bool> ExistsByMerchantReferenceAsync(string merchantReference,CancellationToken cancellationToken = default);

    Task<IEnumerable<Payment>> GetByEnrollmentIdAsync(int enrollmentId,CancellationToken cancellationToken = default);

    void Update(Payment payment);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);


}
