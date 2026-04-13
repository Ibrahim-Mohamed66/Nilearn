namespace Nilearn.Application.Features.Course.DTOs;

public class CourseForUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public string? ThumbnailUrl { get; set; }
}
