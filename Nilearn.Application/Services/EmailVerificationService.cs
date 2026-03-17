using Microsoft.AspNetCore.Identity;
using Nilearn.Application.Common.Enums;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Entities;
using Nilearn.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailTemplateRenderer _templateRenderer;
        private readonly IEmailJobScheduler _emailJobScheduler;
        private readonly Configuration _config;
       

        public EmailVerificationService(
            UserManager<AppUser> userManager,
            IEmailTemplateRenderer templateRenderer,
            IEmailJobScheduler emailJobScheduler,
            Configuration config)
        {
            _userManager = userManager;
            _templateRenderer = templateRenderer;
            _emailJobScheduler = emailJobScheduler;
            _config = config;
        }

        public async Task SendVerificationEmailAsync(AppUser user,CancellationToken cancellationToken)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var verificationLink = $"{_config.FrontendUrl}/confirm/email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            var templateValues = new Dictionary<string, string>
            {
                { "displayName", user.FirstName },
                { "verificationLink", verificationLink },
                { "currentYear", DateTime.UtcNow.Year.ToString() }
            };

            var emailBody = _templateRenderer.Render("Email/Templates/VerifyEmail.html", templateValues);

            await _emailJobScheduler.EnqueueEmailAsync(user.Email, "Verify your email address - Nilearn", emailBody,cancellationToken);
        }

        public async Task<EmailConfirmationResult> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return EmailConfirmationResult.UserNotFound;
            if (user.EmailConfirmed) return EmailConfirmationResult.AlreadyConfirmed;



            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded ? EmailConfirmationResult.Success : EmailConfirmationResult.InvalidToken;
        }
    }
}
