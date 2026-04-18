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
    public string? SecureResourceUrl { get; set; }     
    public string? Format { get; set; }         
    public long? Bytes { get; set; }            // File size in bytes
    public int? DurationInSeconds { get; set; } // Only for videos (from Cloudinary metadata)
    public ResourceType? ResourceType { get; set; }
    public string? Content { get; set; }



    public void UpdateCloudinaryInfo(string publicId, string secureUrl, string format, long bytes, int? durationInSeconds = null)
    {
        CloudinaryPublicId = publicId;
        SecureResourceUrl = secureUrl;
        Format = format;
        Bytes = bytes;
        DurationInSeconds = durationInSeconds;
    }
    public void ClearCloudinaryInfo()
    {
        CloudinaryPublicId = null;
        SecureResourceUrl = null;
        Format = null;
        Bytes = null;
        DurationInSeconds = null;
    }
}
