using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Nilearn.Application.Features.Auth.Login.DTOs
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        [JsonIgnore]
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string[] Roles { get; set; }
    }
}
