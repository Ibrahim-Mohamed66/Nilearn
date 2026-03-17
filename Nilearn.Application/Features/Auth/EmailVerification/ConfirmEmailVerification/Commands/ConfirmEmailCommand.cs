using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Auth.EmailVerification.ConfirmEmailVerification.DTOs;
namespace Nilearn.Application.Features.Auth.EmailVerification.ConfirmEmailVerification.Commands;

public sealed record ConfirmEmailCommand(ConfirmEmailRequestDto emailRequestDto) : IRequest<Result<string>>;
