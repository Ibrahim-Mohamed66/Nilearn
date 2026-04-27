namespace Nilearn.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string? ThumbnailPublicId { get; set; }

    public decimal Price { get; set; }
    public bool IsPublished { get; private set; } = false;
    public DateTime? PublishedAt { get; private set; }

    public int CategoryId { get; set; }
    public int InstructorId { get; set; }

    public Category? Category { get; set; }
    public Instructor? Instructor { get; set; }
    public ICollection<Section> Sections { get; set; } = new List<Section>();
    public ICollection<Enrollment> Enrollments { get; private set; } = new List<Enrollment>();

    public void Publish()
    {
        if (IsPublished) return;
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        if (!IsPublished) return;
        IsPublished = false;
        PublishedAt = null;
    }
}