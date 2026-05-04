using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories;

internal class PlatformWalletRepository : IPlatformWalletRepository
{
    private readonly AppDbContext _context;
    public PlatformWalletRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PlatformWallet wallet, CancellationToken cancellationToken)
    {
       await _context.PlatformWallets.AddAsync(wallet, cancellationToken);
    }

    public async Task<PlatformWallet?> GetAsync(CancellationToken cancellationToken)
    {
        return await _context.PlatformWallets.FirstOrDefaultAsync(cancellationToken);
    }
}

