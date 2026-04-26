using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Lesson.DTOs;

namespace Nilearn.Application.Features.Lesson.Commands.Update.UpdatePdfLesson;

public sealed record UpdatePdfLessonCommand(
    int Id,
    string Title,
    int SectionId,
    string? Description,
    int Order,
    bool IsPreview,
    string UserId,
    FileUpload? PdfFile
) : IRequest<Result<LessonResponse>>;
