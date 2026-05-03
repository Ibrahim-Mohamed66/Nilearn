using System.Text.Json.Serialization;

namespace Nilearn.Application.Common
{
    public class PaymobIntentionResponse
    {
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; } = null!;
        [JsonPropertyName("id")]
        public string IntentionId { get; set; } = null!;
        [JsonPropertyName("order_id")]
        public string OrderId { get; set; } = null!;
    }
}
