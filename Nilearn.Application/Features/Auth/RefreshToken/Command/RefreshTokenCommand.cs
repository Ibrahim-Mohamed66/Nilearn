using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Auth.Login.DTOs;
using Nilearn.Application.Features.Auth.RefreshToken.DTos;

namespace Nilearn.Application.Features.Auth.RefreshToken.Command
{
    public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokenResponseDto>>;

}
