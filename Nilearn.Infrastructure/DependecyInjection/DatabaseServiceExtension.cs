using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nilearn.Infrastructure.Persistence;
using System;

namespace Nilearn.Infrastructure.DependecyInjection
{
    public static class DatabaseServiceExtension
    {
        public static IServiceCollection AddDatabaseServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Get the connection string from appsettings.json
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";

            // Replace the placeholder with the environment variable
            var pgPassword = Environment.GetEnvironmentVariable("PG_PASSWORD") ?? "";
            connectionString = connectionString.Replace("${PG_PASSWORD}", pgPassword);

            // Register DbContext with PostgreSQL
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            return services;
        }
    }
}