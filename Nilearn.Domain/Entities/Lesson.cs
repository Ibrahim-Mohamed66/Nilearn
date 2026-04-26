using Nilearn.Domain.Enums;


namespace Nilearn.Domain.Entities;


public class Lesson : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int Order { get; set; }
    public int SectionId { get; set; }
    public Section Section { get; set; } = null!;

    public string? CloudinaryPublicId { get; set; }        
    public string? Format { get; set; }         
    public long? Bytes { get; set; }            // File size in bytes
    public int? DurationInSeconds { get; set; } // Only for videos (from Cloudinary metadata)
    public LessonType LessonType { get; set; }
    public string? Content { get; set; }
    public bool IsPreview { get; private set; } = false;



    public void UpdateCloudinaryInfo(string publicId, string secureUrl, string format, long bytes, int? durationInSeconds = null)
    {
        CloudinaryPublicId = publicId;
        Format = format;
        Bytes = bytes;
        DurationInSeconds = durationInSeconds;
    }
    public void ClearCloudinaryInfo()
    {
        CloudinaryPublicId = null;
        Format = null;
        Bytes = null;
        DurationInSeconds = null;
    }
    public void MarkAsPreview()
    {
        IsPreview = true;
    }
    public void UnmarkAsPreview()
    {
        IsPreview = false;
    }
}
