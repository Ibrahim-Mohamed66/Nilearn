using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<bool> IsRefreshTokenValidAsync(string refreshToken);
        Task<bool> HasValidRefreshTokenAsync(Guid userId);
        Task AddRefreshTokenAsync(RefreshToken refreshToken,CancellationToken cancellationToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task<AppUser> GetAppUserByTokenAsync(string refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
    }
}
