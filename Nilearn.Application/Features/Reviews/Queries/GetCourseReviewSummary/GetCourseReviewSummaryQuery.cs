using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Reviews.DTOs;

namespace Nilearn.Application.Features.Reviews.Queries.GetCourseReviewSummary;

public sealed record GetCourseReviewSummaryQuery(int CourseId) : IRequest<Result<ReviewSummaryDto>>;
