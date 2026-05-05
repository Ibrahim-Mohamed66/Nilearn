using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Entities;

namespace Nilearn.Application.Features.Auth.ForgotPassword.Commands
{
    internal sealed class ForgotPasswordCommandHandler
        : IRequestHandler<ForgotPasswordCommand, Result<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;
        private readonly IForgotPasswordService _forgotPasswordService;

        public ForgotPasswordCommandHandler(
            UserManager<AppUser> userManager,
            ILogger<ForgotPasswordCommandHandler> logger,
            IForgotPasswordService forgotPasswordService)
        {
            _userManager = userManager;
            _logger = logger;
            _forgotPasswordService = forgotPasswordService;
        }

        public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogWarning(
                    "Password reset requested for non-existent email: {Email} at {UtcNow}",
                    request.Email,
                    DateTime.UtcNow
                );

                return Result<string>.SuccessResponse(
                   message: "If the email exists, a reset link has been sent."
                );
            }

            await _forgotPasswordService.SendResetPasswordEmailAsync(user, cancellationToken);

            _logger.LogInformation(
                "Password reset email successfully queued for user {Email} at {UtcNow}",
                user.Email,
                DateTime.UtcNow
            );

            return Result<string>.SuccessResponse(message:
                "If the email exists, a reset link has been sent."
            );
        }
    }
}