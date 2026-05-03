using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Enums;
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

    public IQueryable<Payment> QueryPaymentHistory(int studentId,PaymentStatus? status = null)
    {
        var query = _context.Payments
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Include(p => p.Enrollment)
            .Where(p => p.Enrollment.StudentId == studentId);
        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }
        return query;
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

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (payment != null)
        {
            payment.IsDeleted = true;
            return true;
        }
        return false;
    }
}
