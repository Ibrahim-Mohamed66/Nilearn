using Microsoft.Extensions.Options;
using Resend;
using Nilearn.Application.Common.Interfaces;

namespace Nilearn.Infrastructure.Email
{
    public class ResendEmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly ResendSettings _settings;

        public ResendEmailService(IResend resend, IOptions<ResendSettings> options)
        {
            _resend = resend;
            _settings = options.Value;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string body,
            CancellationToken cancellationToken)
        {
            var email = new EmailMessage
            {
                From = _settings.FromEmail,
                To = to,
                Subject = subject,
                HtmlBody = body
            };

            var result = await _resend.EmailSendAsync(email, cancellationToken);
            if (result is null)
                throw new Exception("Failed to send email via Resend");
        }
    }
}