using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Features.Reviews.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Reviews.Queries.GetMyReview;

internal sealed class GetMyReviewQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetMyReviewQueryHandler> logger)
    : IRequestHandler<GetMyReviewQuery, Result<ReviewDto>>
{
    public async Task<Result<ReviewDto>> Handle(GetMyReviewQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching review for course {CourseId} by user {UserId}", request.CourseId, request.UserId);

        var student = await unitOfWork.StudentRepository.GetByUserId(request.UserId, cancellationToken);
        if (student is null)
        {
            logger.LogWarning("Student not found for user {UserId}", request.UserId);
            throw new NotFoundException("Student", request.UserId);
        }

        var review = await unitOfWork.ReviewRepository.GetByStudentAndCourseAsync(student.Id, request.CourseId, cancellationToken);
        if (review is null)
        {
            logger.LogInformation("No review found for student {StudentId} on course {CourseId}", student.Id, request.CourseId);
            throw new NotFoundException("Review");
        }

        return Result<ReviewDto>.SuccessResponse(new ReviewDto
        {
            Id = review.Id,
            CourseId = review.CourseId,
            StudentId = review.StudentId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
        }, "Review retrieved successfully.");
    }
}
