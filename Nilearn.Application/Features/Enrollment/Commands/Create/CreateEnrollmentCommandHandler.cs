using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Features.Enrollment.Commands.Create.DTOs;

namespace Nilearn.Application.Features.Enrollment.Commands.Create;

internal sealed class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, Result<CreateEnrollmentResponse>>
{
    private readonly IEnrollmentService _enrollmentService;

    public CreateEnrollmentCommandHandler(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    public Task<Result<CreateEnrollmentResponse>> Handle(
        CreateEnrollmentCommand request,
        CancellationToken cancellationToken)
    {
        return _enrollmentService.CreateEnrollmentAsync(request.UserId, request.CourseId, cancellationToken);
    }
}