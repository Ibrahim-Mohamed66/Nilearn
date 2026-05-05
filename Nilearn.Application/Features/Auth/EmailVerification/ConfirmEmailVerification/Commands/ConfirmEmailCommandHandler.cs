using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Enums;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Features.Auth.EmailVerification.ConfirmEmailVerification.Commands
{
    internal sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<string>>
    {
        private readonly ILogger<ConfirmEmailCommandHandler> _logger;
        private readonly IEmailVerificationService _emailVerificationService;

        public ConfirmEmailCommandHandler(
            ILogger<ConfirmEmailCommandHandler> logger,
            IEmailVerificationService emailVerificationService)
        {
            _logger = logger;
            _emailVerificationService = emailVerificationService;
        }


        public async Task<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            var token = request.Token;

            var result = await _emailVerificationService.ConfirmEmailAsync(userId, token, cancellationToken);

            switch (result)
            {
                case EmailConfirmationResult.Success:
                    _logger.LogInformation("Email confirmed for user {UserId}.", userId);
                    return Result<string>.SuccessResponse("Success", "Email confirmed successfully");

                case EmailConfirmationResult.UserNotFound:
                    _logger.LogWarning("Email confirmation failed for user {UserId}: user not found.", userId);
                    throw new NotFoundException("User", userId);

                case EmailConfirmationResult.InvalidToken:
                    _logger.LogWarning("Email confirmation failed for user {UserId}: invalid or expired token.", userId);
                    throw new BadRequestException("Invalid or expired token");

                case EmailConfirmationResult.AlreadyConfirmed:
                    _logger.LogInformation("Email confirmation skipped for user {UserId}: email already confirmed.", userId);
                    throw new ConflictException("User", "Email already confirmed");

                default:
                    _logger.LogError("Email confirmation failed for user {UserId}: unexpected error occurred.", userId);
                    throw new BadRequestException("An unexpected error occurred");
            }
        }
    }
}
