using Nilearn.Domain.Entities;
namespace Nilearn.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(AppUser user, params string[] roles);
        RefreshToken GenerateRefreshToken();


    }
}
