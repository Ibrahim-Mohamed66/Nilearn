using MediatR;
using Nilearn.Application.Common;

namespace Nilearn.Application.Features.Reviews.Commands.DeleteReview;

public sealed record DeleteReviewCommand(
    string UserId,
    int CourseId) : IRequest<Result<string>>;
