using Nilearn.Application.Common.Enums;

namespace Nilearn.Application.Common.Interfaces;

public interface IMediaService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName,UploadPurpose purpose ,CancellationToken cancellationToken = default);
    Task DeleteImageAsync(string thumbnailPublicId);
    string GetImageUrl(string thumbnailPublicId);
    Task<bool> ImageExistsAsync(string thumbnailPublicId, CancellationToken cancellationToken = default);
}
