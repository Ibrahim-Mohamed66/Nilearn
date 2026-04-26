namespace Nilearn.API.Requests;

public class CreateArticleLessonRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SectionId { get; set; }
    public int Order { get; set; }
    public bool IsPreview { get; set; }
    public string Content { get; set; } = string.Empty;
}
