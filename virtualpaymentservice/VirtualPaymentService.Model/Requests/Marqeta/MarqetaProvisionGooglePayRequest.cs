using System.Text.Json.Serialization;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Requests.Marqeta
{
    public class MarqetaProvisionGooglePayRequest
    {
        [JsonPropertyName("card_token")]
        public string CardToken { get; set; }

        [JsonPropertyName("device_type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeviceType DeviceType { get; set; }

        [JsonPropertyName("provisioning_app_version")]
        public string ProvisioningAppVersion { get; set; }

        [JsonPropertyName("wallet_account_id")]
        public string WalletAccountId { get; set; }

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }
    }
}
