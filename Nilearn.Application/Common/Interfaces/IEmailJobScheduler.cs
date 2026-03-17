using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Common.Interfaces
{
    public interface IEmailJobScheduler
    {
        Task EnqueueEmailAsync(string to, string subject, string body,CancellationToken cancellationToken);
    }
}
