using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Auth.ResetPassword.Commands;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result<string>>;

