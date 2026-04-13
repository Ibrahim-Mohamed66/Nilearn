using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Nilearn.Application.Common.Enums;
using Nilearn.Application.Common.Interfaces;

namespace Nilearn.Infrastructure.Media
{
    internal class MediaService : IMediaService
    {
        private readonly ILogger<MediaService> _logger;
        private readonly Cloudinary _cloudinary;
        public MediaService(Cloudinary cloudinary, ILogger<MediaService> logger)
        {
            _cloudinary = cloudinary;
            _logger = logger;
        }
        public async Task DeleteImageAsync(string thumbnailPublicId)
        {
            if (string.IsNullOrWhiteSpace(thumbnailPublicId))
                throw new ArgumentException("File path cannot be empty", nameof(thumbnailPublicId));

            try
            {
                var deletionParams = new DeletionParams(thumbnailPublicId);
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

        public string GetImageUrl(string thumbnailPublicId)
        {
            if (string.IsNullOrWhiteSpace(thumbnailPublicId))
                return string.Empty;
            var url = _cloudinary.Api.UrlImgUp.BuildUrl(thumbnailPublicId);
            _logger.LogInformation("Generated URL for image {thumbnailPublicId}: {Url}", thumbnailPublicId, url);
            return url;
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


        public async Task<string> UploadImageAsync(Stream imageStream, string fileName, UploadPurpose purpose, CancellationToken cancellationToken = default)
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
                return result.PublicId; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while uploading image {FileName}", fileName);
                throw;
            }
        }
    }
}
