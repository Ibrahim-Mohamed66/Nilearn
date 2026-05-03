using Microsoft.Extensions.DependencyInjection;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register MediatR handlers
            services
                .AddMediatRService()
                .AddFluentValidation()
                .AddEmailApplicationServices();

            services.AddScoped<IEnrollmentService, EnrollmentService>();

            return services;
        }
    }
}
