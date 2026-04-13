using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.Common.Interfaces
{
    public interface IImageJobScheduler
    {
        Task EnqueueDeleteImageAsync(string publicId);
    }
}
