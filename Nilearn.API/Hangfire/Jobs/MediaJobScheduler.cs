using Hangfire;
using Nilearn.Application.Common.Interfaces;

namespace Nilearn.API.Hangfire.Jobs
{
    public class MediaJobScheduler : IMediaJobScheduler
    {
        public Task EnqueueDeleteImageAsync(string publicId)
        {
            BackgroundJob.Enqueue<IMediaService>(x =>
                x.DeleteImageAsync(publicId)
            );

            return Task.CompletedTask;
        }

        public Task EnqueueDeleteVideoAsync(string publicId)
        {
            BackgroundJob.Enqueue<IMediaService>(x =>
                x.DeleteVideoAsync(publicId)
            );

            return Task.CompletedTask;
        }

        public Task EnqueueDeleteDocumentAsync(string publicId)
        {
            BackgroundJob.Enqueue<IMediaService>(x =>
                x.DeleteDocumentAsync(publicId)
            );

            return Task.CompletedTask;
        }
    }
}
