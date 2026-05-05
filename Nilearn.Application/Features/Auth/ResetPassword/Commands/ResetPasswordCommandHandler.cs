using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Enums;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Interfaces;


namespace Nilearn.Application.Features.Auth.ResetPassword.Commands
{
    internal sealed class ResetPasswordCommandHandler: IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        private readonly IForgotPasswordService _forgotPasswordService;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            IForgotPasswordService forgotPasswordService,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _forgotPasswordService = forgotPasswordService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var decodedToken = Uri.UnescapeDataString(request.Token);

            var result = await _forgotPasswordService.ResetPasswordAsync(
                request.Email,
                decodedToken,
                request.NewPassword,
                cancellationToken
            );

            return result switch
            {
                ResetPasswordResult.Success => HandleSuccess(request.Email),
                ResetPasswordResult.UserNotFound => HandleUserNotFound(request.Email),
                ResetPasswordResult.InvalidToken => HandleInvalidToken(request.Email),
                _ => HandleUnknownError(request.Email)
            };
        }

        #region Helpers
        private Result<string> HandleSuccess(string email)
        {
            _logger.LogInformation(
                "Password reset successful for {Email}",
                email
            );

            return Result<string>.SuccessResponse(message: "Password has been reset successfully.");
        }

        private Result<string> HandleUserNotFound(string email)
        {
            _logger.LogWarning(
                "Password reset attempted for non-existent email {Email}",
                email
            );

            // Don't reveal user existence
            return Result<string>.SuccessResponse(message: "Password has been reset successfully.");
        }

        private Result<string> HandleInvalidToken(string email)
        {
            _logger.LogWarning(
                "Invalid or expired reset token used for {Email}",
                email
            );

            throw new BadRequestException("Invalid or expired reset token.");
        }

        private Result<string> HandleUnknownError(string email)
        {
            _logger.LogError(
                "Unknown error occurred during password reset for {Email}",
                email
            );

            throw new BadRequestException("Something went wrong. Please try again later.");
        } 
        #endregion
    }
}