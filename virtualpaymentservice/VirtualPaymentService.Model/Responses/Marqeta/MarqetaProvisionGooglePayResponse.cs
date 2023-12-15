using System;
using System.Text.Json.Serialization;
using VirtualPaymentService.Model.DTO;

namespace VirtualPaymentService.Model.Responses.Marqeta
{
    public class MarqetaProvisionGooglePayResponse
    {
        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; }

        [JsonPropertyName("last_modified_time")]
        public DateTime LastModifiedTime { get; set; }

        [JsonPropertyName("card_token")]
        public string CardToken { get; set; }

        [JsonPropertyName("push_tokenize_request_data")]
        public MarqetaProvisionGooglePayTokenizationData PushTokenizeRequestData { get; set; }
    }
}
