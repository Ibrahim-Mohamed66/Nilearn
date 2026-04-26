using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Lesson.DTOs;

namespace Nilearn.Application.Features.Lesson.Commands.Update.UpdateVideoLesson;

public sealed record UpdateVideoLessonCommand(
    int Id,
    string Title,
    int SectionId,
    string? Description,
    int Order,
    bool IsPreview,
    string UserId,
    FileUpload? VideoFile
) : IRequest<Result<LessonResponse>>;
