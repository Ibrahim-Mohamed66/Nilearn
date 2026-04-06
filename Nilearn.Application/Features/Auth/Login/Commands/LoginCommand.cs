using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Auth.Login.DTOs;

namespace Nilearn.Application.Features.Auth.Login.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponseDto>>;
