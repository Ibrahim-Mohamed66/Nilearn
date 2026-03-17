namespace Nilearn.Application.Features.Auth.EmailVerification.ConfirmEmailVerification.DTOs;

public record ConfirmEmailRequestDto(string UserId, string Token);
