using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Entities;
using Nilearn.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Nilearn.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwt;
        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwt = jwtSettings.Value;
        }
        public string GenerateAccessToken(AppUser user,params string[]roles)
        {
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
               new Claim(JwtRegisteredClaimNames.Name, user.FirstName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in roles) 
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var authSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
            var credentials = new SigningCredentials(authSecurityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpirationMinutes),
                claims: authClaims,
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        

        public RefreshToken GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                ExpiresOn = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationDays),
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}
