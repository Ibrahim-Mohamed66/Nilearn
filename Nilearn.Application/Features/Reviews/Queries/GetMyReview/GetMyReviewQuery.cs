using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Reviews.DTOs;

namespace Nilearn.Application.Features.Reviews.Queries.GetMyReview;

public sealed record GetMyReviewQuery(
    string UserId,
    int CourseId) : IRequest<Result<ReviewDto>>;
