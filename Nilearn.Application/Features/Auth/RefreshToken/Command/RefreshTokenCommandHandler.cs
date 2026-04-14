using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Auth.Login.DTOs;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;


namespace Nilearn.Application.Features.Auth.RefreshToken.Command
{
    public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponseDto>>
    {
        private readonly IJwtService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;
        private readonly UserManager<AppUser> _userManager;
        public RefreshTokenCommandHandler(IJwtService tokenService, IUnitOfWork unitOfWork, ILogger<RefreshTokenCommandHandler> logger, UserManager<AppUser> userManager)
        {
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<Result<LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.RefreshTokenRepository.GetUserByTokenAsync(request.RefreshToken);
            if (user == null)
            {
                _logger.LogWarning("Refresh token not found or no user associated with token: {Token}", request.RefreshToken);
                return Result<LoginResponseDto>.FailureResponse(
                    new List<string> { "Refresh token not found." },
                    "Token refresh failed."
                );
            }
            var userToken =  user.RefreshTokens.FirstOrDefault(rt => rt.Token == request.RefreshToken);
            if (userToken == null || userToken.IsRevoked)
            {
                _logger.LogWarning("Refresh token expired or not found for user: {UserId}", user.Id);
                return Result<LoginResponseDto>.FailureResponse(new List<string> { "Refresh token expired or not found." }, "Token refresh failed.");
            }
            var roles = await _userManager.GetRolesAsync(user);

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newAccessToken = _tokenService.GenerateAccessToken(user,roles.ToArray());
            var refreshTokenEntity = new Nilearn.Domain.Entities.RefreshToken
            {
                Token = newRefreshToken.Token,
                ExpiresOn = newRefreshToken.ExpiresOn,
                UserId = user.Id,
                CreatedOn = DateTime.UtcNow,
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                user.RevokeActiveRefreshTokens();
                await _unitOfWork.RefreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);


                var response = new LoginResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.Token,
                    ExpiresAt = newRefreshToken.ExpiresOn,
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToArray(),
                };

                _logger.LogInformation("Refresh token successful for user: {UserId}", user.Id);

                return Result<LoginResponseDto>.SuccessResponse(response, "Token refreshed successfully.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while refreshing token for user: {UserId}", user.Id);
                 await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                 return Result<LoginResponseDto>.FailureResponse(new List<string> { "An error occurred while refreshing the token." }, "Token refresh failed.");
            }
        }
    }
}
