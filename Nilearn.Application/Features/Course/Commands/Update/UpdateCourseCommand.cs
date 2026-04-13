using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Enums;

namespace Nilearn.Application.Features.Course.Commands.Update;

public sealed record UpdateCourseCommand(
    int CourseId,
    string Title,
    string Description,
    int CategoryId,
    string UserId,
    FileUpload? Thumbnail,
    UploadPurpose Purpose,
    decimal Price,
    bool IsPublished) : IRequest<Result<string>>;
