using Nilearn.Domain.Entities;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;

namespace Nilearn.Application.Common.Interfaces
{
    public interface IPaymentGateway
    {
        Task<PaymentSessionResult> CreatePaymentSessionAsync(PaymentSessionRequest request,CancellationToken cancellationToken = default);
        public PaymentWebhookResult ValidateAndParseWebhook(string payload, string receivedHmac);

    }
}
