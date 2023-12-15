using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Responses.Marqeta
{
    [ExcludeFromCodeCoverage]
    public class MarqetaTransitionCardResponse
    {
        [JsonPropertyName("card_token")]
        public string CardToken { get; set; }

        [JsonPropertyName("user_token")]
        public string LeaseId { get; set; }

        [JsonPropertyName("state")]
        public string CardStatus { get; set; }

        [JsonPropertyName("channel")]
        public static string TransitionSource => "API";

        [JsonPropertyName("created_time")]
        public DateTime CreateTransitionTime { get; set; }

        [JsonPropertyName("card_product_token")]
        public string CardProductToken { get; set; }

        [JsonPropertyName("expiration_time")]
        public DateTime ExpirationTime { get; set; }
    }
}
