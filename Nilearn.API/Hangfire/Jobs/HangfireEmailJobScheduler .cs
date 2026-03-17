using Hangfire;
using Nilearn.Application.Common.Interfaces;

namespace Nilearn.API.Hangfire.Jobs
{
    public class HangfireEmailJobScheduler : IEmailJobScheduler
    {
        private readonly IEmailService _emailService;

        public HangfireEmailJobScheduler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task EnqueueEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
        {
            BackgroundJob.Enqueue(() => _emailService.SendEmailAsync(to, subject, body,cancellationToken));
            return Task.CompletedTask;
        }
    }
}

