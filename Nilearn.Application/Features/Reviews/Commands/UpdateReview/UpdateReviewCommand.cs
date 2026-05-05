using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Reviews.DTOs;

namespace Nilearn.Application.Features.Reviews.Commands.UpdateReview;

public sealed record UpdateReviewCommand(
    string UserId,
    int CourseId,
    int Rating,
    string Comment) : IRequest<Result<ReviewDto>>;
