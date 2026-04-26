namespace Nilearn.Application.Common;

public class MediaUploadResult
{
    public string PublicId { get; set; } = null!;
    public string Format { get; set; } = null!;
    public long Bytes { get; set; }
    public int? DurationInSeconds { get; set; }
}
