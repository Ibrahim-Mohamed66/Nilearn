using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nilearn.Application.Common;
using Nilearn.Application.Common.Exceptions;
using Nilearn.Application.Common.Interfaces;
using Nilearn.Infrastructure.Payments.Paymob.Models;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


namespace Nilearn.Infrastructure.Payments.Paymob;

public sealed class PaymobGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly PaymobSettings _settings;
    private readonly ILogger<PaymobGateway> _logger;

    public PaymobGateway(
        HttpClient httpClient,
        IOptions<PaymobSettings> settings,
        ILogger<PaymobGateway> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    // ---------------- CREATE PAYMENT ----------------
    public async Task<PaymentSessionResult> CreatePaymentSessionAsync(
        PaymentSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var amountCents = ToCents(request.Amount);
        var currency = request.Currency;
        var paymentMethods = new []{ _settings.CardIntegrationId, _settings.WalletIntegrationId };
        var billingData = new
        {
            first_name = request.FirstName ?? "N/A",
            last_name = request.LastName ?? "N/A",
            phone_number = request.PhoneNumber ?? "N/A",
            email = request.Email ?? "N/A",
            country = "EG",
        };

        var payload = new
        {
            payment_methods = paymentMethods,
            amount = amountCents,
            billing_data = billingData,
            currency = currency,
            items = new[]
            {
                new
                {
                    name = request.CourseTitle,
                    amount = ToCents(request.CoursePrice),
                    quantity = 1

                }
            },
            extras = new
            {
                merchant_reference_id = request.MerchantReferenceId,
                enrollment_id = request.EnrollmentId
            },
            expiration = _settings.PaymentKeyExpirationSeconds

        };

        
        var response = await _httpClient.PostAsJsonAsync("/v1/intention/", payload, cancellationToken);


        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Paymob request failed. Status: {Status}, Error: {Error}",
                response.StatusCode, error);

            throw  new PaymentGatewayException(
                $"Paymob API error ({(int)response.StatusCode}): {error}",
                (int)response.StatusCode);
        }

        var result = await response.Content
            .ReadFromJsonAsync<PaymobIntentionResponse>(cancellationToken: cancellationToken)
            ?? throw new PaymentGatewayException("Paymob returned an empty response body.");

        if (string.IsNullOrWhiteSpace(result.ClientSecret))
            throw new PaymentGatewayException("Paymob returned a response with a missing or empty client_secret.");

        _logger.LogInformation("Paymob session created. IntentionId: {Id}, OrderId: {OrderId}",
                result.IntentionId, result.OrderId);

        return new PaymentSessionResult
        {
            Provider = "Paymob",
            PaymobIntentionId = result.IntentionId,
            PaymobOrderId = result.OrderId,
            PaymentUrl = BuildPaymentUrl(result.ClientSecret),
        };
    }
    public PaymentWebhookResult ValidateAndParseWebhook(string payload, string receivedHmac)
    {
        ValidateHmac(payload, receivedHmac);
        return ParseWebhook(payload);
    }

    // Kept internal so the controller can't accidentally skip HMAC validation.
    internal PaymentWebhookResult ParseWebhook(string payload)
    {
        using var doc = JsonDocument.Parse(payload);
        var root = doc.RootElement;

        if (!root.TryGetProperty("obj", out var obj))
            throw new PaymentGatewayException("Invalid Paymob webhook: missing 'obj' property.");

        var success = obj.GetProperty("success").GetBoolean();
        var pending = obj.GetProperty("pending").GetBoolean();



        if (!obj.TryGetProperty("id", out var idProp))
            throw new PaymentGatewayException("Missing transaction id");

        var transactionId = idProp.GetInt64();

        if (!obj.TryGetProperty("order", out var order) ||
            !order.TryGetProperty("id", out var orderIdProp))
            throw new PaymentGatewayException("Missing order id");

        var orderId = orderIdProp.GetInt64();

        // Use Int64 — large transactions exceed Int32.MaxValue in cents
        var amountCents = obj.GetProperty("amount_cents").GetInt64();
        var currency = obj.GetProperty("currency").GetString()!;
        if (string.IsNullOrWhiteSpace(currency))
            throw new PaymentGatewayException("Missing currency");

        string? responseCode = null;
        string? message = null;
        string? merchantReferenceId = null;
        string? enrollmentId = null;

        if (obj.TryGetProperty("data", out var data))
        {
            data.TryGetProperty("txn_response_code", out var code);
            responseCode = code.ValueKind != JsonValueKind.Undefined ? code.GetString() : null;

            data.TryGetProperty("message", out var msg);
            message = msg.ValueKind != JsonValueKind.Undefined ? msg.GetString() : null;
        }

        // Recover the extras we sent during intention creation
        if (obj.TryGetProperty("payment_key_claims", out var claims) && claims.TryGetProperty("extra", out var extra))
        {
            if (extra.TryGetProperty("merchant_reference_id", out var mrid))
                merchantReferenceId = mrid.GetString();

            if (extra.TryGetProperty("enrollment_id", out var eid) && eid.TryGetInt32(out var id))
            {
                enrollmentId = id.ToString();
            }
        }

        _logger.LogInformation("Webhook processed: OrderId={OrderId}, Success={Success}, Amount={Amount}",orderId,success, amountCents);

        return new PaymentWebhookResult
        {
            IsSuccess = success && !pending,
            TransactionId = transactionId,
            OrderId = orderId,
            Amount = amountCents / 100m,
            Currency = currency,
            ResponseCode = responseCode,
            Message = message,
            MerchantReferenceId = merchantReferenceId,
            EnrollmentId = enrollmentId
        };
    }

    internal void ValidateHmac(string payload, string receivedHmac)
    {
        using var doc = JsonDocument.Parse(payload);
        var root = doc.RootElement;

        if (!root.TryGetProperty("obj", out var obj))
            throw new PaymentGatewayException("Invalid Paymob webhook: missing 'obj' property.");

        string GetProp(JsonElement el, string propName)
        {
            if (el.ValueKind != JsonValueKind.Undefined && el.TryGetProperty(propName, out var p) && p.ValueKind != JsonValueKind.Null)
            {
                return p.ValueKind switch
                {
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    JsonValueKind.Number => p.GetRawText(),
                    JsonValueKind.String => p.GetString() ?? "",
                    _ => p.GetRawText()
                };
            }
            return "";
        }

        var sourceData = obj.TryGetProperty("source_data", out var sd) ? sd : default;
        var order = obj.TryGetProperty("order", out var o) ? o : default;

        var concatenatedString =
            $"{GetProp(obj, "amount_cents")}" +
            $"{GetProp(obj, "created_at")}" +
            $"{GetProp(obj, "currency")}" +
            $"{GetProp(obj, "error_occured")}" +
            $"{GetProp(obj, "has_parent_transaction")}" +
            $"{GetProp(obj, "id")}" +
            $"{GetProp(obj, "integration_id")}" +
            $"{GetProp(obj, "is_3d_secure")}" +
            $"{GetProp(obj, "is_auth")}" +
            $"{GetProp(obj, "is_capture")}" +
            $"{GetProp(obj, "is_refunded")}" +
            $"{GetProp(obj, "is_standalone_payment")}" +
            $"{GetProp(obj, "is_voided")}" +
            $"{GetProp(order, "id")}" +
            $"{GetProp(obj, "owner")}" +
            $"{GetProp(obj, "pending")}" +
            $"{GetProp(sourceData, "pan")}" +
            $"{GetProp(sourceData, "sub_type")}" +
            $"{GetProp(sourceData, "type")}" +
            $"{GetProp(obj, "success")}";

        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_settings.HmacSecret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedString));
        var computedHmac = Convert.ToHexString(hashBytes).ToLowerInvariant();

        var normalizedReceived = receivedHmac?.ToLowerInvariant() ?? "";

        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computedHmac),
                Encoding.UTF8.GetBytes(normalizedReceived)))
        {
            _logger.LogWarning("Invalid HMAC signature received.");
            throw new UnauthorizedAccessException("Invalid webhook signature.");
        }
    }

    private static long ToCents(decimal amount)
       => (long)Math.Round(amount * 100, MidpointRounding.AwayFromZero);

    private string BuildPaymentUrl(string clientSecret)
        => $"{_settings.BaseUrl}/unifiedcheckout/?publicKey={Uri.EscapeDataString(_settings.PublicKey)}&clientSecret={Uri.EscapeDataString(clientSecret)}";
}
