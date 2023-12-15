using System.Text.Json.Serialization;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Requests.Marqeta
{
    public class MarqetaProvisionApplePayRequest
    {
        [JsonPropertyName("card_token")]
        public string CardToken { get; set; }

        [JsonPropertyName("device_type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DeviceType DeviceType { get; set; }

        [JsonPropertyName("provisioning_app_version")]
        public string ProvisioningAppVersion { get; set; }

        [JsonPropertyName("certificates")]
        public string[] Certificates { get; set; }

        [JsonPropertyName("nonce")]
        public string Nonce { get; set; }

        [JsonPropertyName("nonce_signature")]
        public string NonceSignature { get; set; }

        public MarqetaProvisionApplePayRequest()
        {
            Certificates = new string[2];
        }
    }
}
