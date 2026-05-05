using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Infrastructure.Email;
using Resend;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Infrastructure.DependencyInjection
{
    public static class EmailServiceExtension
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddHttpClient<ResendClient>();
            services.Configure<ResendClientOptions>(o =>
            {
                o.ApiToken = configuration.GetSection("ResendSettings")["ApiKey"] ?? string.Empty;
            });
            services.AddTransient<IResend, ResendClient>();

            services.Configure<ResendSettings>(configuration.GetSection("ResendSettings"));
            services.AddTransient<IEmailService, ResendEmailService>();
            services.AddTransient<IEmailTemplateRenderer, EmailTemplateRenderer>();
            
            return services;
        }
    }
}
