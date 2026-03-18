using MediatR;
using Nilearn.Application.Common;
namespace Nilearn.Application.Features.Auth.ForgotPassword.Commands;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result<string>>;
