using MediatR;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Extensions;
using Nilearn.Application.Features.Reviews.DTOs;
using Nilearn.Domain.Interfaces;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Reviews.Queries.GetCourseReviews;

internal sealed class GetCourseReviewsQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetCourseReviewsQueryHandler> logger)
    : IRequestHandler<GetCourseReviewsQuery, Result<PagedResponse<ReviewDto>>>
{
    public async Task<Result<PagedResponse<ReviewDto>>> Handle(
        GetCourseReviewsQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching reviews for course {CourseId}: Page {Page}, Size {Size}",
            request.CourseId, request.PageNumber, request.PageSize);

        var course = await unitOfWork.CourseRepository.GetByIdAsync(request.CourseId, cancellationToken);
            if (course is null)
            {
                logger.LogWarning("Course not found: {CourseId}", request.CourseId);
                throw new NotFoundException("Course", request.CourseId);
            }

            var query = unitOfWork.ReviewRepository.QueryByCourseId(request.CourseId);

            var projectedQuery = query.Select(r => new ReviewDto
            {
                Id = r.Id,
                CourseId = r.CourseId,
                StudentId = r.StudentId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                StudentName = (r.Student!.AppUser!.FirstName ?? "") + " " + (r.Student.AppUser.LastName ?? "")
            });

        var pagedReviews = await projectedQuery.ToPagedAsync(request.PageNumber, request.PageSize, cancellationToken);

        logger.LogInformation("Successfully retrieved {Count} reviews for course {CourseId} out of {TotalCount}",
            pagedReviews.Items.Count, request.CourseId, pagedReviews.TotalCount);

        return Result<PagedResponse<ReviewDto>>.SuccessResponse(pagedReviews, "Reviews retrieved successfully.");
    }
}
