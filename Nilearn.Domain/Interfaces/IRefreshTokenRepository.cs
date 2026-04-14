using Nilearn.Domain.Entities;

namespace Nilearn.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<bool> IsValidAsync(string refreshToken,CancellationToken cancellationToken = default);
        Task<bool> HasValidAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshToken refreshToken,CancellationToken cancellationToken = default);
        Task RevokeAsync(string refreshToken,CancellationToken cancellationToken = default);
        Task<AppUser> GetUserByTokenAsync(string refreshToken,CancellationToken cancellationToken = default);
        Task<RefreshToken> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
