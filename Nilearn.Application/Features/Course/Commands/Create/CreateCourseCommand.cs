using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Enums;
using Nilearn.Application.Features.Course.DTOs;


namespace Nilearn.Application.Features.Course.Commands.Create;

public sealed record CreateCourseCommand(
    string Title,
    string Description,
    int CategoryId,
    string UserId,
    FileUpload Thumbnail,
    UploadPurpose Purpose,
    decimal Price,
    bool IsPublished) : IRequest<Result<CreateCourseResponse>>;
