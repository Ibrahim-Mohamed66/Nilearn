using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nilearn.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nilearn.Infrastructure.DependecyInjection;

public static class DependecyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,IConfiguration configuration)
    {
        return services.AddDatabaseServices(configuration);
    }
}
