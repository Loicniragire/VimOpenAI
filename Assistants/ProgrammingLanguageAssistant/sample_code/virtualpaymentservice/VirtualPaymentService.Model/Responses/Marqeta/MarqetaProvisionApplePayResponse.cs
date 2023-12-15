using System;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Responses.Marqeta
{
    public class MarqetaProvisionApplePayResponse
    {
        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; }

        [JsonPropertyName("last_modified_time")]
        public DateTime LastModifiedTime { get; set; }

        [JsonPropertyName("card_token")]
        public string CardToken { get; set; }

        [JsonPropertyName("encrypted_pass_data")]
        public string EncryptedPassData { get; set; }

        [JsonPropertyName("activation_data")]
        public string ActivationData { get; set; }

        [JsonPropertyName("ephemeral_public_key")]
        public string EphemeralPublicKey { get; set; }
    }
}
