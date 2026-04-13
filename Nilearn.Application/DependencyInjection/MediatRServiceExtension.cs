using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nilearn.Application.Common.Behaviors;

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
