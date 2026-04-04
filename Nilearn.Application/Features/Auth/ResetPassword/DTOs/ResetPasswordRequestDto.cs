namespace Nilearn.Application.Features.Auth.ResetPassword.DTOs
{
    public record ResetPasswordRequestDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }

    }
}
