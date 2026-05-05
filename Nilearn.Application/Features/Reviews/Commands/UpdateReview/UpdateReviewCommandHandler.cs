using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Features.Reviews.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Reviews.Commands.UpdateReview;

internal sealed class UpdateReviewCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateReviewCommandHandler> logger)
    : IRequestHandler<UpdateReviewCommand, Result<ReviewDto>>
{
    public async Task<Result<ReviewDto>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating review for course {CourseId} by user {UserId}", request.CourseId, request.UserId);

        var student = await unitOfWork.StudentRepository.GetByUserId(request.UserId, cancellationToken);
        if (student is null)
        {
            logger.LogWarning("Student not found for user {UserId}", request.UserId);
            throw new NotFoundException("Student", request.UserId);
        }

        // Find the student's review for this course (tracked for update)
        var review = await unitOfWork.ReviewRepository.GetByStudentAndCourseAsync(student.Id, request.CourseId, cancellationToken);
        if (review is null)
        {
            logger.LogWarning("Review not found for student {StudentId} on course {CourseId}", student.Id, request.CourseId);
            throw new NotFoundException("Review");
        }

        // Ensure only owner can update
        if (review.StudentId != student.Id)
        {
            logger.LogWarning("Unauthorized update attempt on review {ReviewId} by student {StudentId}", review.Id, student.Id);
            throw new ForbiddenAccessException("You are not authorized to update this review.");
        }

        // Use domain method to update
        review.Update(request.Rating, request.Comment);
        unitOfWork.ReviewRepository.Update(review);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Review {ReviewId} updated successfully", review.Id);

        return Result<ReviewDto>.SuccessResponse(new ReviewDto
        {
            Id = review.Id,
            CourseId = review.CourseId,
            StudentId = review.StudentId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
        }, "Review updated successfully.");
    }
}
