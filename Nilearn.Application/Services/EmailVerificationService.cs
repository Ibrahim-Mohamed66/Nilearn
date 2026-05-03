using Microsoft.AspNetCore.Identity;
using Nilearn.Application.Common.Enums;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Entities;
using Nilearn.Shared.Models;


namespace Nilearn.Application.Services
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailTemplateRenderer _templateRenderer;
        private readonly IEmailJobScheduler _emailJobScheduler;
        private readonly AppConfiguration _config;
       

        public EmailVerificationService(
            UserManager<AppUser> userManager,
            IEmailTemplateRenderer templateRenderer,
            IEmailJobScheduler emailJobScheduler,
            AppConfiguration config)
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
            if (result.Succeeded)
            {
                await SendWelcomeEmailAsync(user, cancellationToken);
                return EmailConfirmationResult.Success;
            }
            return EmailConfirmationResult.InvalidToken;
        }

        private async Task SendWelcomeEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            var loginLink = $"{_config.FrontendUrl}/login";

            var templateValues = new Dictionary<string, string>
            {
                { "displayName", user.FirstName },
                { "loginLink", loginLink },
                { "currentYear", DateTime.UtcNow.Year.ToString() }
            };

            var emailBody = _templateRenderer.Render("Email/Templates/Welcome.html", templateValues);

            await _emailJobScheduler.EnqueueEmailAsync(user.Email, "Welcome to Nilearn!", emailBody, cancellationToken);
        }

        public async Task SendEnrollmentActivatedEmailAsync(string studentEmail, string studentFirstName, string courseTitle, string instructorName, int courseId, CancellationToken cancellationToken)
        {
            var courseLink = $"{_config.FrontendUrl}/course/{courseId}";

            var templateValues = new Dictionary<string, string>
            {
                { "displayName", studentFirstName },
                { "courseTitle", courseTitle },
                { "instructorName", instructorName },
                { "courseLink", courseLink },
                { "currentYear", DateTime.UtcNow.Year.ToString() }
            };

            var emailBody = _templateRenderer.Render("Email/Templates/EnrollmentActivated.html", templateValues);

            await _emailJobScheduler.EnqueueEmailAsync(
                studentEmail,
                $"You're Enrolled: {courseTitle}",
                emailBody,
                cancellationToken);
        }
    }
}
