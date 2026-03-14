using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nilearn.Domain.Entities;
using Nilearn.Infrastructure.Persistence;

namespace Nilearn.Infrastructure.DependencyInjection;

public static class IdentityServiceExtension
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
        {
            //// Password settings
            //options.Password.RequireDigit = true;
            //options.Password.RequiredLength = 6;
            //options.Password.RequireUppercase = true;
            //options.Password.RequireLowercase = true;
            //options.Password.RequireNonAlphanumeric = false;

            //// Lockout settings
            //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            //options.Lockout.MaxFailedAccessAttempts = 5;
            //options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}