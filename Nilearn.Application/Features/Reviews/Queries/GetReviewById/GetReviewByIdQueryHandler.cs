using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Features.Reviews.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Reviews.Queries.GetReviewById;

internal sealed class GetReviewByIdQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetReviewByIdQueryHandler> logger)
    : IRequestHandler<GetReviewByIdQuery, Result<ReviewDto>>
{
    public async Task<Result<ReviewDto>> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching review {ReviewId}", request.ReviewId);

        var review = await unitOfWork.ReviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
        {
            logger.LogWarning("Review not found: {ReviewId}", request.ReviewId);
            throw new NotFoundException("Review", request.ReviewId);
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
