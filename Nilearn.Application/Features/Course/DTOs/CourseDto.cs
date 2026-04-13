namespace Nilearn.Application.Features.Course.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }

    public string CategoryName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; }
}