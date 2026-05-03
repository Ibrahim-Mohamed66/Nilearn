namespace Nilearn.Infrastructure.Payments.Paymob.Models;

public class PaymobSettings
{
    public string SecretKey { get; set; } = null!;
    public string PublicKey { get; set; } = null!;
    public string HmacSecret { get; set; } = null!;
    public int CardIntegrationId { get; set; }
    public int WalletIntegrationId { get; set; }
    public string BaseUrl { get; set; } = null!;
    public int PaymentKeyExpirationSeconds { get; set; }
}