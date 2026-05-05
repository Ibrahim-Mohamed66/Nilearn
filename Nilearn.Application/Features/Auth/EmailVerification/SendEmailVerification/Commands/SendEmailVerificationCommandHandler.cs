using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Entities;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Auth.EmailVerification.SendEmailVerification.Commands
{
    internal sealed class SendEmailVerificationCommandHandler : IRequestHandler<SendEmailVerificationCommand, Result<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly ILogger<SendEmailVerificationCommandHandler> _logger;

        public SendEmailVerificationCommandHandler(
            UserManager<AppUser> userManager,
            IEmailVerificationService emailVerificationService,
            ILogger<SendEmailVerificationCommandHandler> logger)
        {
            _userManager = userManager;
            _emailVerificationService = emailVerificationService;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("Verification email failed for user {UserId}: user not found.", request.UserId);
                throw new NotFoundException("User", request.UserId);
            }

            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Verification email skipped for user {UserId}: email already confirmed.", request.UserId);
                throw new ConflictException("User", "Email already confirmed");
            }

            await _emailVerificationService.SendVerificationEmailAsync(user, cancellationToken);

            _logger.LogInformation("Verification email sent successfully for user {UserId}.", request.UserId);
            return Result<string>.SuccessResponse("Success", "Verification email sent successfully");
        }
    }
}