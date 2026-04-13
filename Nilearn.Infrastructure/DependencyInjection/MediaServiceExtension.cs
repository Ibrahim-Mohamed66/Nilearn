using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Infrastructure.Media;

namespace Nilearn.Infrastructure.DependencyInjection;

public static class MediaServiceExtension
{
    public static IServiceCollection AddMediaServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MediaSettings>(
            configuration.GetSection(MediaSettings.SectionName));

        // Register Cloudinary client as singleton
        services.AddSingleton<Cloudinary>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MediaSettings>>().Value;

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret);

            return new Cloudinary(account) { Api = { Secure = true } };
        });

       
        services.AddScoped<IMediaService, MediaService>();

        return services;
    }
}