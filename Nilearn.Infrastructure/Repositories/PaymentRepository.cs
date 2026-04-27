using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories;

internal class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;
    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Payment payment, CancellationToken cancellationToken = default)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
    }

    public async Task<bool> ExistsByMerchantReferenceAsync(string merchantReferenceId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
            .AnyAsync(p => p.MerchantReferenceId == merchantReferenceId, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> GetByEnrollmentIdAsync(int enrollmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
           .AsNoTracking()
           .Where(p => p.EnrollmentId == enrollmentId)
           .ToListAsync(cancellationToken);
    }

    public async Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
        .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);   
    }
    

    public async Task<Payment?> GetByMerchantReferenceAsync(string merchantReferenceId, CancellationToken cancellationToken = default)
    {
        return await _context.Payments
        .FirstOrDefaultAsync(p => p.MerchantReferenceId == merchantReferenceId, cancellationToken);
    }

    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
    }
}
