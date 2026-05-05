using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Features.Reviews.DTOs;
using Nilearn.Domain.Interfaces;

namespace Nilearn.Application.Features.Reviews.Queries.GetCourseReviewSummary;

internal sealed class GetCourseReviewSummaryQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetCourseReviewSummaryQueryHandler> logger)
    : IRequestHandler<GetCourseReviewSummaryQuery, Result<ReviewSummaryDto>>
{
    public async Task<Result<ReviewSummaryDto>> Handle(
        GetCourseReviewSummaryQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching review summary for course {CourseId}", request.CourseId);

        var course = await unitOfWork.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course is null)
            {
                logger.LogWarning("Course not found: {CourseId}", request.CourseId);
                throw new NotFoundException("Course", request.CourseId);
            }

            var reviewsQuery = unitOfWork.ReviewRepository.QueryByCourseId(request.CourseId);

            var totalReviews = await reviewsQuery.CountAsync(cancellationToken);

            if (totalReviews == 0)
            {
                return Result<ReviewSummaryDto>.SuccessResponse(new ReviewSummaryDto
                {
                    CourseId = request.CourseId,
                    AverageRating = 0,
                    TotalReviews = 0
                }, "No reviews found for this course.");
            }

            var averageRating = await reviewsQuery.AverageAsync(r => r.Rating, cancellationToken);

            // Rating distribution: group by rating and count
            var distribution = await reviewsQuery
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var ratingDistribution = new Dictionary<int, int>
            {
                { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }
            };

            foreach (var item in distribution)
            {
                ratingDistribution[item.Rating] = item.Count;
            }

            var summary = new ReviewSummaryDto
            {
                CourseId = request.CourseId,
                AverageRating = Math.Round(averageRating, 2),
                TotalReviews = totalReviews,
                RatingDistribution = ratingDistribution
            };

            logger.LogInformation("Review summary for course {CourseId}: Avg={Avg}, Total={Total}",
                request.CourseId, summary.AverageRating, summary.TotalReviews);

        return Result<ReviewSummaryDto>.SuccessResponse(summary, "Review summary retrieved successfully.");
    }
}
