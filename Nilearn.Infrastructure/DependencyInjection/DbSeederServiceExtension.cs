using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nilearn.Infrastructure.Initializer;

namespace Nilearn.Infrastructure.DependencyInjection
{
    public static class DbSeederServiceExtension
    {
        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<IDbInitializer>>();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            try
            {
                await dbInitializer.InitializeAsync();
                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database seeding failed.");
            }
        }
    }
}
