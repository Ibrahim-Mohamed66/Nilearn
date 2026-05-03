using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Enrollment.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Enrollment.Queries.IsEnrolled;

internal sealed class IsEnrolledQueryHandler
    : IRequestHandler<IsEnrolledQuery, Result<IsEnrolledResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<IsEnrolledQueryHandler> _logger;

    public IsEnrolledQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<IsEnrolledQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<IsEnrolledResponse>> Handle(
        IsEnrolledQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking enrollment for user {UserId} in course {CourseId}",
            request.UserId, request.CourseId);

        try
        {
            var student = await _unitOfWork.StudentRepository.GetByUserId(request.UserId, cancellationToken);
            if (student is null)
            {
                _logger.LogWarning("Student not found for UserId: {UserId}", request.UserId);
                return Result<IsEnrolledResponse>.SuccessResponse(
                    new IsEnrolledResponse { Enrolled = false },
                    "User is not a student");
            }

            var enrollment = await _unitOfWork.EnrollmentRepository
                .GetByStudentAndCourseAsync(student.Id, request.CourseId, cancellationToken);

            if (enrollment is null)
            {
                return Result<IsEnrolledResponse>.SuccessResponse(
                    new IsEnrolledResponse { Enrolled = false },
                    "Student is not enrolled in this course");
            }

            return Result<IsEnrolledResponse>.SuccessResponse(
                new IsEnrolledResponse
                {
                    Enrolled = enrollment.IsActive,
                    Status = enrollment.Status.ToString(),
                    EnrollmentId = enrollment.Id
                },
                "Enrollment status retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking enrollment for user {UserId} in course {CourseId}",
                request.UserId, request.CourseId);
            return Result<IsEnrolledResponse>.FailureResponse(message:"Failed to check enrollment status");
        }
    }
}
