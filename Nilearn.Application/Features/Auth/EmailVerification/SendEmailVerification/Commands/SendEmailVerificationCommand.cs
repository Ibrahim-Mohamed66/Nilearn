using MediatR;
using Nilearn.Application.Common;


namespace Nilearn.Application.Features.Auth.EmailVerification.SendEmailVerification.Commands;

public sealed record SendEmailVerificationCommand(string UserId) : IRequest<Result<string>>;
