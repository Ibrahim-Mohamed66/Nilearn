using Hangfire;
using Nilearn.Application.Common.Interfaces;

namespace Nilearn.API.Hangfire.Jobs
{
    public class ImageJobScheduler : IImageJobScheduler
    {
        public Task EnqueueDeleteImageAsync(string publicId)
        {
            BackgroundJob.Enqueue<IMediaService>(x =>
                x.DeleteImageAsync(publicId)
            );

            return Task.CompletedTask;
        }
    }
}
