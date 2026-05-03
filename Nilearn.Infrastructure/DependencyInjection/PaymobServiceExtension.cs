using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Infrastructure.Payments.Paymob;
using Nilearn.Infrastructure.Payments.Paymob.Models;
using System.Net.Http.Headers;

namespace Nilearn.Infrastructure.DependencyInjection;

public static class PaymobServiceExtension
{
    private const string PaymobSectionName = "Paymob";

    public static IServiceCollection AddPaymobService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PaymobSettings>(configuration.GetSection(PaymobSectionName));

        services.AddHttpClient<IPaymentGateway, PaymobGateway>((sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<PaymobSettings>>().Value;

            if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                throw new InvalidOperationException("Paymob BaseUrl is missing from configuration.");
            }

            client.BaseAddress = new Uri(settings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token",settings.SecretKey);
        });

        return services;
    }
}