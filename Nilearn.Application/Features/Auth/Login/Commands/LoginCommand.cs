using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Auth.Login.DTOs;
namespace Nilearn.Application.Features.Auth.Login.Commands;

public sealed record LoginCommand(LoginRequestDto loginRequestDto) : IRequest<Result<LoginResponseDto>>;
