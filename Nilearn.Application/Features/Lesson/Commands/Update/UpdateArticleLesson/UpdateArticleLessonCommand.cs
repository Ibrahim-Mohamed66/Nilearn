using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Lesson.DTOs;

namespace Nilearn.Application.Features.Lesson.Commands.Update.UpdateArticleLesson;

public sealed record UpdateArticleLessonCommand(
    int Id,
    string Title,
    int sectionId,
    string? Description,
    int Order,
    bool IsPreview,
    string UserId,
    string Content
) : IRequest<Result<LessonResponse>>;

