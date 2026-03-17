using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body,CancellationToken cancellationToken);
    }
}
