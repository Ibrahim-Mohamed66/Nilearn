using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Application.DependencyInjection
{
    public static class MediatRServiceExtension
    {
        public static IServiceCollection AddMediatRService(this IServiceCollection services)
        {
            services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssemblies(typeof(ApplicationAssemblyMarker).Assembly);
            });
            return services;
        }
    }
}
