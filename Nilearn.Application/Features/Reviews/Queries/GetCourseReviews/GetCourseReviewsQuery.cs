using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Reviews.DTOs;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Reviews.Queries.GetCourseReviews;

public sealed record GetCourseReviewsQuery(
    int CourseId,
    int PageNumber,
    int PageSize) : IRequest<Result<PagedResponse<ReviewDto>>>;
