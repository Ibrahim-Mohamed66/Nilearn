using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;
using Nilearn.Application.Common.Exceptions;

namespace Nilearn.Application.Features.Auth.Register.Instructor.Commands
{
    internal sealed class RegisterInstructorCommandHandler : IRequestHandler<RegisterInstructorCommand, Result<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterInstructorCommandHandler> _logger;
        private readonly IEmailVerificationService _emailVerificationService;
        public RegisterInstructorCommandHandler(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            ILogger<RegisterInstructorCommandHandler> logger,
            IEmailVerificationService emailVerificationService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _emailVerificationService = emailVerificationService;
        }


        public async Task<Result<string>> Handle(RegisterInstructorCommand request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                _logger.LogWarning("Registration failed: email {Email} is already in use.", request.Email);
                throw new ConflictException("User", "Email is already in use.");
            }

            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Registration failed for {Email}: {Errors}",
                            user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new BadRequestException("Registration failed", result.Errors.Select(e => e.Description));
            }

            var instructorProfile = new Nilearn.Domain.Entities.Instructor
            {
                AppUserId = user.Id,
                Bio = request.Bio,
                Headline = request.Headline,
                WebsiteUrl = request.WebsiteUrl,
            };
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                await _unitOfWork.InstructorRepository.AddAsync(instructorProfile, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _userManager.AddToRoleAsync(user, Role.Instructor.ToString());
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

            // try
            // {
            //     await _emailVerificationService.SendVerificationEmailAsync(user, cancellationToken);

            // }
            // catch (Exception ex)
            // {
            //     _logger.LogError(ex, "Failed to send verification email for user {UserId}.", user.Id);
            //     // — user can request resend
            // }

            return Result<string>.SuccessResponse(
                "Success",
                "User registered successfully."
            );


        }
    }
}
