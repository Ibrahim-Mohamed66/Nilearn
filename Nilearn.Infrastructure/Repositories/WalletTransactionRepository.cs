using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories;

internal class WalletTransactionRepository : IWalletTransactionRepository
{
    private readonly AppDbContext _context;
    public WalletTransactionRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(WalletTransaction transaction, CancellationToken cancellationToken)
    {
        await _context.WalletTransactions.AddAsync(transaction, cancellationToken);
    }

    public async Task<bool> ExistsByPaymentAndInstructorAsync(int paymentId, int? instructorId, CancellationToken cancellationToken)
    {
        return await _context.WalletTransactions.AnyAsync(t => t.PaymentId == paymentId && t.InstructorId == instructorId, cancellationToken);
    }

    public Task<WalletTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.WalletTransactions.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task<List<WalletTransaction>> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken)
    {
        return _context.WalletTransactions.Where(t => t.InstructorId == instructorId).ToListAsync(cancellationToken);
    }

    public Task<List<WalletTransaction>> GetByPaymentIdAsync(int paymentId, CancellationToken cancellationToken)
    {
        return _context.WalletTransactions.Where(t => t.PaymentId == paymentId).ToListAsync(cancellationToken);
    }
}
