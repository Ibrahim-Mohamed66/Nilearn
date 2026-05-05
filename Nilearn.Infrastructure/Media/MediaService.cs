    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.Extensions.Logging;
    using Nilearn.Application.Common;
    using Nilearn.Application.Common.Enums;
    using Nilearn.Application.Common.Interfaces;

    namespace Nilearn.Infrastructure.Media;

    internal class MediaService : IMediaService
    {
        private readonly ILogger<MediaService> _logger;
        private readonly Cloudinary _cloudinary;
        public MediaService(Cloudinary cloudinary, ILogger<MediaService> logger)
        {
            _cloudinary = cloudinary;
            _logger = logger;
        }

        public async Task DeleteDocumentAsync(string documentPublicId)
        {
            if(string.IsNullOrWhiteSpace(documentPublicId))
                throw new ArgumentException("PublicId cannot be empty", nameof(documentPublicId));

            var deletionParams = new DeletionParams(documentPublicId)
            {
                ResourceType = ResourceType.Raw
            };
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result == "ok" || result.Result == "not_found")
            {
                _logger.LogInformation("Document deleted successfully: {documentPublicId}", documentPublicId);
            }
            else
            {
                _logger.LogError("Failed to delete document {documentPublicId}: {Error}", documentPublicId, result.Error?.Message);
                throw new InvalidOperationException($"Failed to delete document: {result.Error?.Message}");
            }

        }

        public async Task DeleteImageAsync(string thumbnailPublicId)
        {
            if (string.IsNullOrWhiteSpace(thumbnailPublicId))
                throw new ArgumentException("File path cannot be empty", nameof(thumbnailPublicId));

            try
            {
                var deletionParams = new DeletionParams(thumbnailPublicId)
                {
                    ResourceType = ResourceType.Image
                };
                var result = await _cloudinary.DestroyAsync(deletionParams);

                if (result.Result == "ok" || result.Result == "not_found")
                {
                    _logger.LogInformation("Image deleted successfully: {thumbnailPublicId}", thumbnailPublicId);
                }
                else
                {
                    _logger.LogError("Failed to delete image {thumbnailPublicId}: {Error}", thumbnailPublicId, result.Error?.Message);
                    throw new InvalidOperationException($"Failed to delete image: {result.Error?.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while deleting image {thumbnailPublicId}", thumbnailPublicId);
                throw;
            }
        }

        public async Task DeleteVideoAsync(string videoPublicId)
        {
            if(string.IsNullOrWhiteSpace(videoPublicId))
                throw new ArgumentException("PublicId cannot be empty", nameof(videoPublicId));

            var deletionParams = new DeletionParams(videoPublicId)
            {
                ResourceType = ResourceType.Video
            };
            var result = await _cloudinary.DestroyAsync(deletionParams);
            if (result.Result == "ok" || result.Result == "not_found")
            {
                _logger.LogInformation("Video deleted successfully: {videoPublicId}", videoPublicId);
            }
            else
            {
                _logger.LogError("Failed to delete video {videoPublicId}: {Error}", videoPublicId, result.Error?.Message);
                throw new InvalidOperationException($"Failed to delete video: {result.Error?.Message}");
            }
        }
        public string GetDocumentUrl(string documentPublicId)
        {
            if (string.IsNullOrWhiteSpace(documentPublicId))
                return string.Empty;
            var url = _cloudinary.Api.Url.Secure(true).BuildUrl(documentPublicId);
            _logger.LogInformation("Generated URL for document {documentPublicId}: {Url}", documentPublicId, url);
            return url;
        }

        public string GetImageUrl(string thumbnailPublicId)
        {
            if (string.IsNullOrWhiteSpace(thumbnailPublicId))
                return string.Empty;
            var url = _cloudinary.Api.UrlImgUp.Secure(true).BuildUrl(thumbnailPublicId);
            _logger.LogInformation("Generated URL for image {thumbnailPublicId}: {Url}", thumbnailPublicId, url);
            return url;
        }

        public string GetVideoThumbnailUrl(string videoPublicId)
        {
            string thumbnailUrl = _cloudinary.Api.UrlVideoUp
                        .Transform(new Transformation()
                            .Width(300)
                            .Height(300)
                            .Crop("thumb")
                            .Gravity("auto")
                            .StartOffset("auto") 
                        )
                        .Format("jpg")
                        .Secure(true)
                        .BuildUrl(videoPublicId);
            return thumbnailUrl;
        }

        public string GetVideoUrl(string videoPublicId)
        {
            if (!string.IsNullOrWhiteSpace(videoPublicId))
            {
                var url = _cloudinary.Api.UrlVideoUp.ResourceType("video").Secure(true).BuildUrl(videoPublicId);
                _logger.LogInformation("Generated URL for video {videoPublicId}: {Url}", videoPublicId, url);
                return url;
            }
            else
            {
                _logger.LogWarning("Attempted to generate video URL with empty PublicId");
                return string.Empty;
            }

        }

        public async Task<bool> ImageExistsAsync(string thumbnailPublicId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(thumbnailPublicId))
                return false;

            try
            {
                var result = await _cloudinary.GetResourceAsync(new GetResourceParams(thumbnailPublicId), cancellationToken);
                var exists = result.StatusCode == System.Net.HttpStatusCode.OK;
                _logger.LogInformation("Checked existence for image {thumbnailPublicId}: {Exists}", thumbnailPublicId, exists);
                return exists;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while checking existence of image {thumbnailPublicId}", thumbnailPublicId);
                throw;
            }
        }

        public async Task<MediaUploadResult> UploadDocumentAsync(Stream documentStream, string fileName, CancellationToken cancellationToken = default)
        {
            if (documentStream == null || !documentStream.CanRead || documentStream.Length == 0)
                throw new ArgumentException("Document stream cannot be empty", nameof(documentStream));

            const string folder = "documents";
            fileName = Path.GetFileName(fileName);
            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, documentStream),
                    Folder = folder,
                    UseFilename = true,
                    UniqueFilename = true,
                   
                };
                var result = await _cloudinary.UploadAsync(uploadParams,cancellationToken: cancellationToken);
                if (result != null && result.StatusCode == System.Net.HttpStatusCode.OK && result.Error == null)
                {
                    _logger.LogInformation("Document uploaded successfully: {FileName} → {PublicId}", fileName, result.PublicId);
                    return new MediaUploadResult
                    {
                        PublicId = result.PublicId,
                        Format = result.Format,
                        Bytes = result.Bytes

                    };
                }
                else
                {
                    var errorMsg = result?.Error?.Message ?? "No response from Cloudinary";
                    _logger.LogError("Document upload failed for {FileName}: {Error}", fileName, errorMsg);
                    throw new InvalidOperationException($"Document upload failed: {errorMsg}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while uploading document {FileName}", fileName);

                throw new InvalidOperationException($"Document upload failed: {ex.Message}", ex);
            }
        }

        public async Task<MediaUploadResult> UploadImageAsync(Stream imageStream, string fileName, UploadPurpose purpose, CancellationToken cancellationToken = default)
        {
            if (imageStream == null || imageStream.Length == 0)
                throw new ArgumentException("Image stream cannot be empty", nameof(imageStream));

            // Map UploadPurpose to folder names
            string folder = purpose switch
            {
                UploadPurpose.ProfilePicture => "profiles",
                UploadPurpose.Thumbnail => "thumbnails",
                _ => "others"
            };

            try
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, imageStream),
                    Folder = folder,
                    Transformation = new Transformation().Width(800).Height(800).Crop("limit")
                };

                var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("Image upload failed for {FileName}: {Error}", fileName, result.Error?.Message);
                    throw new InvalidOperationException($"Image upload failed: {result.Error?.Message}");
                }

                _logger.LogInformation("Image uploaded successfully: {FileName} → {PublicId}", fileName, result.PublicId);
                var mediaResult = new MediaUploadResult
                {
                    PublicId = result.PublicId,
                    Format = result.Format,
                    Bytes = result.Bytes
                };
                return mediaResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while uploading image {FileName}", fileName);
                throw;
            }
        }

        public async Task<MediaUploadResult> UploadVideoAsync(Stream videoStream, string fileName, CancellationToken cancellationToken = default)
        {
            if (videoStream == null || !videoStream.CanRead)
                throw new ArgumentException("Video stream cannot be empty", nameof(videoStream));

            const string folder = "videos";
       
            fileName = Path.GetFileName(fileName);
            try
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(fileName, videoStream),
                    Folder = folder,                   
                    EagerAsync = true,



                };
                const int chunkSize = 20 * 1024 * 1024; // 20 MB
                var result = await _cloudinary.UploadLargeAsync(uploadParams, chunkSize, cancellationToken: cancellationToken);
                if (result != null && result.StatusCode == System.Net.HttpStatusCode.OK && result.Error == null)
                {
                    _logger.LogInformation("Video uploaded successfully: {FileName} → {PublicId}", fileName, result.PublicId);
                    return new MediaUploadResult
                    {
                        PublicId = result.PublicId,
                        Format = result.Format,
                        Bytes = result.Bytes,
                        DurationInSeconds = (int)Math.Round(result.Duration),   
                    };

                }
                else
                {
                    var errorMsg = result?.Error?.Message ?? "No response from Cloudinary";
                    _logger.LogError("Video upload failed for {FileName}: {Error}", fileName, errorMsg);
                    throw new InvalidOperationException($"Video upload failed: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while uploading video {FileName}", fileName);
                throw new InvalidOperationException($"Video upload failed: {ex.Message}", ex);
            }
        }
    }

