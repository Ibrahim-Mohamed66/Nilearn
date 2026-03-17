using MediatR;
using Nilearn.Application.Common;
namespace Nilearn.Application.Features.Auth.Logout.Commands;

public sealed record LogoutCommand(string token) : IRequest<Result<string>>;

