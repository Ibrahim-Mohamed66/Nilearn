using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Common.Interfaces
{
    public interface IMediaJobScheduler
    {
        Task EnqueueDeleteImageAsync(string publicId);
        Task EnqueueDeleteVideoAsync(string publicId);
        Task EnqueueDeleteDocumentAsync(string publicId);
    }
}
