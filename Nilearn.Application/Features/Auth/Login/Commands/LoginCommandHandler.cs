using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Auth.Login.DTOs;
using Nilearn.Domain.Entities;
using Nilearn.Shared.Models;
using System.Data;

namespace Nilearn.Application.Features.Auth.Login.Commands
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtService _tokenService;
        private readonly JwtSettings _jwt;
        private readonly ILogger<LoginCommandHandler> _logger;
        public LoginCommandHandler(UserManager<AppUser> userManager, IJwtService tokenService, IOptions<JwtSettings> jwt, ILogger<LoginCommandHandler> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _jwt = jwt.Value;
            _logger = logger;
        }
        public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
           var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Email == request.loginRequestDto.Email, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Login attempt failed for email: {Email}", request.loginRequestDto.Email);
                return Result<LoginResponseDto>.FailureResponse(new List<string> { "Invalid email or password." }, "Login failed.");
            }
            
            var passwordValid = await _userManager.CheckPasswordAsync(user, request.loginRequestDto.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Login attempt failed for email: {Email}", request.loginRequestDto.Email);
                return Result<LoginResponseDto>.FailureResponse(new List<string> { "Invalid email or password." }, "Login failed.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles.ToArray());

            user.RevokeActiveRefreshTokens();

            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                _logger.LogError("Failed to update user {UserId} with new refresh token. Errors: {Errors}", user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                return Result<LoginResponseDto>.FailureResponse(
                    updateResult.Errors.Select(e => e.Description).ToList(),
                    "Login failed.");
            }
            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpirationMinutes), // read from settings
                UserId = user.Id,
                Email = user.Email,
                Roles = roles.ToArray(),
            };
            _logger.LogInformation("User {Email} logged in successfully.", user.Email);
            return Result<LoginResponseDto>.SuccessResponse(response, "Login successful.");
        }
    }
}
