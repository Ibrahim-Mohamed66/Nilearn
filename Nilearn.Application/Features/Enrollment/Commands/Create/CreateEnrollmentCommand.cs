using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Enrollment.Commands.Create.DTOs;

namespace Nilearn.Application.Features.Enrollment.Commands.Create;

public sealed record CreateEnrollmentCommand(string UserId, int CourseId) : IRequest<Result<CreateEnrollmentResponse>>;
