namespace Nilearn.Application.Features.Reviews.DTOs;

public class ReviewDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? StudentName { get; set; }
}
