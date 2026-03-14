using System.Text.Json.Serialization;

namespace Nilearn.Application.Features.Auth.RefreshToken.DTos
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
