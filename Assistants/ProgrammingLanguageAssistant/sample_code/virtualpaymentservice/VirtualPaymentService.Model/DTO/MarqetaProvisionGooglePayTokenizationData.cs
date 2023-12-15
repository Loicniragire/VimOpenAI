using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.DTO
{
    public class MarqetaProvisionGooglePayTokenizationData
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("last_digits")]
        public string LastDigits { get; set; }

        [JsonPropertyName("network")]
        public string Network { get; set; }

        [JsonPropertyName("token_service_provider")]
        public string TokenServiceProvider { get; set; }

        [JsonPropertyName("opaque_payment_card")]
        public string OpaquePaymentCard { get; set; }

        [JsonPropertyName("user_address")]
        public MarqetaProvisionGooglePayUserAddress UserAddress { get; set; }
    }
}
