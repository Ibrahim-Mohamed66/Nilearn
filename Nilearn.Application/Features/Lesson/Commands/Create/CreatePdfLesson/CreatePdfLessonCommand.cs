using MediatR;
using Nilearn.Application.Common;
using Nilearn.Application.Features.Lesson.DTOs;


namespace Nilearn.Application.Features.Lesson.Commands.Create.CreatePdfLesson;

public sealed record CreatePdfLessonCommand(string Title,
string? Description,
int SectionId,
int Order,
bool IsPreview,
string UserId,
FileUpload PdfFile) : IRequest<Result<LessonResponse>>;

