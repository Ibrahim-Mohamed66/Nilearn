using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories;

internal class InstructorWalletRepository : IInstructorWalletRepository
{

    private readonly AppDbContext _context;
    public InstructorWalletRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(InstructorWallet wallet, CancellationToken cancellationToken)
    {
        await _context.InstructorWallets.AddAsync(wallet, cancellationToken);
    }

    public Task<InstructorWallet?> GetByInstructorIdAsync(int instructorId, CancellationToken cancellationToken)
    {
        return _context.InstructorWallets.FirstOrDefaultAsync(w => w.InstructorId == instructorId, cancellationToken);
    }
}
