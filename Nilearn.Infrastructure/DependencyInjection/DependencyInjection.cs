using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nilearn.Infrastructure.Initializer;
using Nilearn.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDatabaseServices(configuration)
            .AddIdentityServices(configuration)
            .AddEmailService(configuration)
            .AddJwtServices(configuration);

        services.AddScoped<IDbInitializer, DbInitializer>();
          
        return services;


    }
}
