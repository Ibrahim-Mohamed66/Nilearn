using Nilearn.Domain.Enums;

namespace Nilearn.Application.Features.Lesson.DTOs;

public record LessonResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int Order { get; set; }
    public string? Description { get; set; }
    public int SectionId { get; set; }

    public LessonType LessonType { get; set; }

    public bool IsLocked { get; set; }
    public string? PdfUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? ArticleContent { get; set; }
}
