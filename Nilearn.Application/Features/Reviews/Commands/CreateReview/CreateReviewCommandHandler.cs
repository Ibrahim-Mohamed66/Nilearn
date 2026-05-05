using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Features.Reviews.DTOs;
using Nilearn.Domain.Entities;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Reviews.Commands.CreateReview;

internal sealed class CreateReviewCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateReviewCommandHandler> logger)
    : IRequestHandler<CreateReviewCommand, Result<ReviewDto>>
{
    public async Task<Result<ReviewDto>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating review for course {CourseId} by user {UserId}", request.CourseId, request.UserId);

        var student = await unitOfWork.StudentRepository.GetByUserId(request.UserId, cancellationToken);
        if (student is null)
        {
            logger.LogWarning("Student not found for user {UserId}", request.UserId);
            throw new NotFoundException("Student", request.UserId);
        }

        var course = await unitOfWork.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);
        if (course is null)
        {
            logger.LogWarning("Course not found: {CourseId}", request.CourseId);
            throw new NotFoundException("Course", request.CourseId);
        }

        // Check enrollment — student must be enrolled to review
        var isEnrolled = await unitOfWork.EnrollmentRepository.IsEnrolledAsync(student.Id, request.CourseId, cancellationToken);
        if (!isEnrolled)
        {
            logger.LogWarning("Student {StudentId} is not enrolled in course {CourseId}", student.Id, request.CourseId);
            throw new ForbiddenAccessException("You must be enrolled in this course to submit a review.");
        }

        // Prevent duplicate review (unique constraint: student + course)
        var existingReview = await unitOfWork.ReviewRepository.GetByStudentAndCourseAsync(student.Id, request.CourseId, cancellationToken);
        if (existingReview is not null)
        {
            logger.LogWarning("Duplicate review attempt by student {StudentId} for course {CourseId}", student.Id, request.CourseId);
            throw new ConflictException("Review", "You have already reviewed this course.");
        }

        var review = new Review(request.CourseId, student.Id, request.Rating, request.Comment);

        await unitOfWork.ReviewRepository.AddAsync(review, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Review {ReviewId} created successfully for course {CourseId}", review.Id, request.CourseId);

        return Result<ReviewDto>.SuccessResponse(new ReviewDto
        {
            Id = review.Id,
            CourseId = review.CourseId,
            StudentId = review.StudentId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
        }, "Review created successfully.");
    }
}
