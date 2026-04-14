using Microsoft.EntityFrameworkCore;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken refreshToken,CancellationToken cancellationToken)
        {
            await _context.AddAsync(refreshToken,cancellationToken);
        }

        public async Task<AppUser?> GetUserByTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
           var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            var user = await _context.Users.Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Id == token.Id));

            return user;
        }

        public async Task<RefreshToken> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken =default)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            return token;
        }

        public async Task<bool> HasValidAsync(Guid userId ,CancellationToken cancellationToken = default) 
        {
            return await _context.RefreshTokens
                .Where(r => r.UserId == userId)
                .WhereActive()
                .AnyAsync();

        }

        public async Task<bool> IsValidAsync(string refreshToken,CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
              .WhereActive()
              .AnyAsync(rt => rt.Token == refreshToken);
        }

        public async Task RevokeAsync(string refreshToken , CancellationToken cancellationToken = default)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if(token != null)
            {
                token.Revoke();
            }
            
        }
    }
}
public static class RefreshTokenExtensions
{
    public static IQueryable<RefreshToken> WhereActive(this IQueryable<RefreshToken> query)
    {
        return query.Where(r => !r.IsRevoked && r.ExpiresOn > DateTime.UtcNow);
    }
}
