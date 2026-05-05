using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Reviews.Commands.DeleteReview;

internal sealed class DeleteReviewCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteReviewCommandHandler> logger)
    : IRequestHandler<DeleteReviewCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting review for course {CourseId} by user {UserId}", request.CourseId, request.UserId);

        // Resolve student from authenticated user
        var student = await unitOfWork.StudentRepository.GetByUserId(request.UserId, cancellationToken);
        if (student is null)
        {
            logger.LogWarning("Student not found for user {UserId}", request.UserId);
            throw new NotFoundException("Student", request.UserId);
        }

        // Find the student's review for this course
        var review = await unitOfWork.ReviewRepository.GetByStudentAndCourseAsync(student.Id, request.CourseId, cancellationToken);
        if (review is null)
        {
            logger.LogWarning("Review not found for student {StudentId} on course {CourseId}", student.Id, request.CourseId);
            throw new NotFoundException("Review");
        }

        // Ensure only owner can delete
        if (review.StudentId != student.Id)
        {
            logger.LogWarning("Unauthorized delete attempt on review {ReviewId} by student {StudentId}", review.Id, student.Id);
            throw new ForbiddenAccessException("You are not authorized to delete this review.");
        }

        // Soft delete
        var deleted = await unitOfWork.ReviewRepository.DeleteAsync(review.Id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("Review", review.Id);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Review {ReviewId} soft-deleted successfully", review.Id);

        return Result<string>.SuccessResponse(message: "Review deleted successfully.");
    }
}
