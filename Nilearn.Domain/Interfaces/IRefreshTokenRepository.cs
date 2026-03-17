using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<bool> IsRefreshTokenValidAsync(string refreshToken,CancellationToken cancellationToken = default);
        Task<bool> HasValidRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddRefreshTokenAsync(RefreshToken refreshToken,CancellationToken cancellationToken = default);
        Task RevokeRefreshTokenAsync(string refreshToken,CancellationToken cancellationToken = default);
        Task<AppUser> GetAppUserByTokenAsync(string refreshToken,CancellationToken cancellationToken = default);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
