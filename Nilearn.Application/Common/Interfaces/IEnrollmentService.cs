using Nilearn.Application.Common;
using Nilearn.Application.Features.Enrollment.Commands.Create.DTOs;

namespace Nilearn.Application.Common.Interfaces;

public interface IEnrollmentService
{
    Task<Result<CreateEnrollmentResponse>> CreateEnrollmentAsync(string userId, int courseId, CancellationToken cancellationToken = default);
}
