using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Auth.EmailVerification.ConfirmEmailVerification.Commands;

public sealed record ConfirmEmailCommand(string UserId, string Token) : IRequest<Result<string>>;
