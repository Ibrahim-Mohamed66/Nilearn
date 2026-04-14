namespace Nilearn.Domain.Entities;

public class Section : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public int Order { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }
}
