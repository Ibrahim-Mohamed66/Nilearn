using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Auth.EmailVerification.SendEmailVerification.Commands;
using Nilearn.Application.Services;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Auth.Register.Student.Commands
{
    internal sealed class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, Result<string>>
    {
        private readonly ILogger<RegisterStudentCommandHandler> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailVerificationService _emailVerificationService;

        public RegisterStudentCommandHandler(
            ILogger<RegisterStudentCommandHandler> logger,
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IEmailVerificationService emailVerificationService)
        {
            _logger = logger;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailVerificationService = emailVerificationService;
        }

        public async Task<Result<string>> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                _logger.LogWarning("Registration failed: email {Email} is already in use.", request.Email);
                return Result<string>.FailureResponse(
                    new List<string> { "Email is already in use." }, "Email is already in use.");
            }

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Registration failed for {Email}: {Errors}",
                    user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return Result<string>.FailureResponse(
                    result.Errors.Select(e => e.Description).ToList(), "Registration failed");
            }

            var studentProfile = new Nilearn.Domain.Entities.Student
            {
                AppUserId = user.Id,
                CurrentLevel = request.CurrentLevel,
                StudentNumber = request.StudentNumber
            };

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                await _unitOfWork.StudentRepository.AddAsync(studentProfile, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _userManager.AddToRoleAsync(user, Role.Student.ToString());
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("User {Email} registered successfully.", user.Email);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                await _userManager.DeleteAsync(user);
                _logger.LogError(ex, "Registration transaction failed for {Email}.", user.Email);
                throw;
            }

            try
            {
                await _emailVerificationService.SendVerificationEmailAsync(user, cancellationToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email for user {UserId}.", user.Id);
                // — user can request resend
            }

            return Result<string>.SuccessResponse(
                "Success",
                "User registered successfully. Please check your email to verify your account."
            );
        }
    }
}