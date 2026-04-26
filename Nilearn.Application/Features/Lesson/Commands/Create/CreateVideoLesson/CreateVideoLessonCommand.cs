

using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Lesson.DTOs;

namespace Nilearn.Application.Features.Lesson.Commands.Create.CreateVideoLesson;

public sealed record CreateVideoLessonCommand(string Title,
    string? Description,
    int SectionId,
    int Order,
    bool IsPreview,
    string UserId,
    FileUpload VideoFile) : IRequest<Result<LessonResponse>>;
