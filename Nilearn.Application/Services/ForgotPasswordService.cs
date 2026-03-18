using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common.Enums;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Entities;
using Nilearn.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nilearn.Application.Services
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailTemplateRenderer _templateRenderer;
        private readonly IEmailJobScheduler _emailJobScheduler;
        private readonly AppConfiguration _config;
        private readonly ILogger<ForgotPasswordService> _logger;

        public ForgotPasswordService(
            UserManager<AppUser> userManager,
            IEmailTemplateRenderer templateRenderer,
            IEmailJobScheduler emailJobScheduler,
            AppConfiguration config,
            ILogger<ForgotPasswordService> logger)
        {
            _userManager = userManager;
            _templateRenderer = templateRenderer;
            _emailJobScheduler = emailJobScheduler;
            _config = config;
            _logger = logger;
        }

       
        public async Task SendResetPasswordEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{_config.FrontendUrl}/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";

                var templateValues = new Dictionary<string, string>
                {
                    { "displayName", user.FirstName },
                    { "resetLink", resetLink },
                    { "expirationTimeHours", "24" },
                    { "currentYear", DateTime.UtcNow.Year.ToString() }
                };

                var emailBody = _templateRenderer.Render("Email/Templates/ResetPassword.html", templateValues);

                await _emailJobScheduler.EnqueueEmailAsync(
                    user.Email,
                    "Reset your password - Nilearn",
                    emailBody,
                    cancellationToken
                );

                _logger.LogInformation(
                    "Password reset email successfully queued for user {Email} at {Time}",
                    user.Email,
                    DateTime.UtcNow
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to queue password reset email for user {Email} at {Time}",
                    user.Email,
                    DateTime.UtcNow
                );
                throw;
            }
        }

        public async Task<ResetPasswordResult> ResetPasswordAsync(
            string email,
            string token,
            string newPassword,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("Password reset attempted for non-existent email {Email} at {Time}", email, DateTime.UtcNow);
                return ResetPasswordResult.UserNotFound;
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation("Password successfully reset for user {Email} at {Time}", email, DateTime.UtcNow);
                return ResetPasswordResult.Success;
            }
            else
            {
                var errors = string.Join(", ", result.Errors);
                _logger.LogWarning(
                    "Password reset failed for user {Email} at {Time}. Errors: {Errors}",
                    email,
                    DateTime.UtcNow,
                    errors
                );
                return ResetPasswordResult.InvalidToken;
            }
        }
    }
}