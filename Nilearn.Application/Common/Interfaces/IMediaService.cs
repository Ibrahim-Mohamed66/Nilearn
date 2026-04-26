using Nilearn.Application.Common.Enums;

namespace Nilearn.Application.Common.Interfaces;

public interface IMediaService
{
    Task<MediaUploadResult> UploadImageAsync(Stream imageStream, string fileName,UploadPurpose purpose ,CancellationToken cancellationToken = default);
    Task DeleteImageAsync(string thumbnailPublicId);
    string GetImageUrl(string thumbnailPublicId);
    Task<bool> ImageExistsAsync(string thumbnailPublicId, CancellationToken cancellationToken = default);


    Task<MediaUploadResult> UploadVideoAsync(Stream videoStream, string fileName, CancellationToken cancellationToken = default);
    Task DeleteVideoAsync(string videoPublicId);

    Task<MediaUploadResult> UploadDocumentAsync(Stream documentStream, string fileName, CancellationToken cancellationToken = default);
    Task DeleteDocumentAsync(string documentPublicId);
    string GetVideoThumbnailUrl(string videoPublicId);
    string GetVideoUrl(string videoPublicId);
    string GetDocumentUrl(string documentPublicId);
}
    