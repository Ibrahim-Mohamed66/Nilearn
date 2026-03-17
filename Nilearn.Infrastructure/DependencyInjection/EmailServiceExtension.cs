using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Infrastructure.Email;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Infrastructure.DependencyInjection
{
    public static class EmailServiceExtension
    {
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IEmailTemplateRenderer, EmailTemplateRenderer>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            return services;
        }
    }
}
