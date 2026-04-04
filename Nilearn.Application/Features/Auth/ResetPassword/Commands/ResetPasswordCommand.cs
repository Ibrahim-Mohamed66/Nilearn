using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Auth.ResetPassword.Commands;
public record ResetPasswordCommand( string email, string token, string newPassword) : IRequest<Result<string>>;

