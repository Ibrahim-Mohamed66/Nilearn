using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nilearn.Domain.Interfaces;
using Nilearn.Infrastructure.Initializer;
using Nilearn.Infrastructure.Persistence;
using Nilearn.Infrastructure.Repositories;


namespace Nilearn.Infrastructure.DependencyInjection
{
    public static class DatabaseServiceExtension
    {
        public static IServiceCollection AddDatabaseServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Get the connection string directly
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";

            services.AddScoped<TimestampInterceptor>();
            services.AddScoped<DomainEventInterceptor>();

            // Register DbContext with PostgreSQL
            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString);
                options.AddInterceptors(
                    sp.GetRequiredService<TimestampInterceptor>(),
                    sp.GetRequiredService<DomainEventInterceptor>());
            });
            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
