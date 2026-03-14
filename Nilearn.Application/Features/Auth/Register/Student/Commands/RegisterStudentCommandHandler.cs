using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nilearn.Application.Common;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Enums;
using Nilearn.Domain.Interfaces;
using Nilearn.Shared.Models;


namespace Nilearn.Application.Features.Auth.Register.Student.Commands
{
    public sealed class RegisterStudentCommandHandler : IRequestHandler<RegisterStudentCommand, Result<string>>
    {
        private readonly ILogger<RegisterStudentCommandHandler> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly JwtSettings _jwt;
        private readonly IUnitOfWork _unitOfWork;
        public RegisterStudentCommandHandler(ILogger<RegisterStudentCommandHandler> logger, UserManager<AppUser> userManager, IOptions<JwtSettings> jwt, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _userManager = userManager;
            _jwt = jwt.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<string>> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.registerRequestDto.Email) != null)
                return Result<string>.FailureResponse(
                    new List<string> { "Email is already in use." }, "Registration failed.");

            var user = new AppUser
            {
                UserName = request.registerRequestDto.Email,
                Email = request.registerRequestDto.Email,
                FirstName = request.registerRequestDto.FirstName,
                LastName = request.registerRequestDto.LastName,
                DateOfBirth = request.registerRequestDto.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
            };
            var result = await _userManager.CreateAsync(user, request.registerRequestDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Role.Student.ToString());
                _logger.LogInformation("User registered successfully with email: {Email}", user.Email);
                var studentProfile = new Nilearn.Domain.Entities.Student
                {
                    AppUserId = user.Id,
                    CurrentLevel = 1,
                    StudentNumber = request.registerRequestDto.StudentNumber,
                };

                try
                {
                    await _unitOfWork.BeginTransactionAsync(cancellationToken);
                    await _unitOfWork.StudentRepository.AddAsync(studentProfile, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    _logger.LogInformation("Student profile created successfully for user with email: {Email}", user.Email);

                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    _logger.LogError(ex, "Failed to create student profile for user with email: {Email}", user.Email);
                    await _userManager.DeleteAsync(user);
                    throw;
                }


                return Result<string>.SuccessResponse("User registered successfully.", "Registration successful.");


            }
            else
            {
                _logger.LogError("User registration failed for email: {Email} - Errors: {Errors}", user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return Result<string>.FailureResponse(result.Errors.Select(e => e.Description).ToList(), "Registration failed.");
            }



        }
    }
}
