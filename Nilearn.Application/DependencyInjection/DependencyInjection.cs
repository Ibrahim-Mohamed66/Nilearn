using Microsoft.Extensions.DependencyInjection;
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
                .AddFluentValidation();

            return services;
        }
    }
}
