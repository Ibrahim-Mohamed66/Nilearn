using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace Nilearn.Application.DependencyInjection
{
    public static class ValidatorServiceExtension
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
           services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
            return services;
        }
            
    }
}
