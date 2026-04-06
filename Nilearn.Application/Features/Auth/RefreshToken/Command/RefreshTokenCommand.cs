using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Auth.Login.DTOs;

namespace Nilearn.Application.Features.Auth.RefreshToken.Command;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<LoginResponseDto>>;
