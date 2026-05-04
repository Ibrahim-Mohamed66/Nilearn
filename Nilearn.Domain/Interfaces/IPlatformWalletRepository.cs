using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces;

public interface IPlatformWalletRepository
{
    Task<PlatformWallet?> GetAsync(CancellationToken cancellationToken);
    Task AddAsync(PlatformWallet wallet, CancellationToken cancellationToken);
}