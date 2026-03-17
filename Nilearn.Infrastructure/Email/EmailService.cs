using Microsoft.Extensions.Options;
using Nilearn.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Nilearn.Infrastructure.Email
{
    internal class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
        {
            using var smtp = new SmtpClient(_settings.SmtpServer)
            {
                Port = _settings.SmtpPort,
                UseDefaultCredentials = false,  
                Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword),
                EnableSsl = _settings.EnableSsl
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            await smtp.SendMailAsync(message,cancellationToken);
        }
    }
}
