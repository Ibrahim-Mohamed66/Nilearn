using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Reviews.DTOs;

namespace Nilearn.Application.Features.Reviews.Queries.GetReviewById;

public sealed record GetReviewByIdQuery(int ReviewId) : IRequest<Result<ReviewDto>>;
