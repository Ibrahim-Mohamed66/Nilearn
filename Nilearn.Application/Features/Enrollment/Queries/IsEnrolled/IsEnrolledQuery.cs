using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Enrollment.DTOs;

namespace Nilearn.Application.Features.Enrollment.Queries.IsEnrolled;

public sealed record IsEnrolledQuery(
    string UserId,
    int CourseId) : IRequest<Result<IsEnrolledResponse>>;
