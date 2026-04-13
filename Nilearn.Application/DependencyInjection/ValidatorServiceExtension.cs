using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nilearn.Application.Common.Behaviors;


namespace Nilearn.Application.DependencyInjection
{
    public static class ValidatorServiceExtension
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection services)
        {
           services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
           services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
            
    }
}
