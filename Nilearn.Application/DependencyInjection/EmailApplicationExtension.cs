using Microsoft.Extensions.DependencyInjection;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Services;

namespace Nilearn.Application.DependencyInjection
{
    public static class EmailApplicationExtension
    {
        public static IServiceCollection AddEmailApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            return services;
        }
    }
}
