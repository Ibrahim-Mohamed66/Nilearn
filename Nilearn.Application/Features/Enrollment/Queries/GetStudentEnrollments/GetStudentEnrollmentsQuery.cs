using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Enrollment.DTOs;
using Nilearn.Domain.Enums;
using Nilearn.Shared.Models;

namespace Nilearn.Application.Features.Enrollment.Queries.GetStudentEnrollments;

public sealed record GetStudentEnrollmentsQuery(
    string UserId,
    int PageNumber,
    int PageSize,
    EnrollmentStatus? Status = null) : IRequest<Result<PagedResponse<EnrollmentDto>>>;
