using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Requests.Marqeta
{
    /// <summary>
    /// Contract for Marqeta API https://www.marqeta.com/docs/core-api/card-transitions#postCardtransitions
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaTransitionCardRequest
    {
        /// <summary>
        /// The source of the card transition.
        /// Hardcoded since this will always come from the API.
        /// </summary>
        [JsonPropertyName("channel")]
#pragma warning disable CA1822 // Mark members as static. This would exclude the property from the POST body.
        public string TransitionSource => "API";
#pragma warning restore CA1822 // Mark members as static

        [JsonPropertyName("card_token")]
        public string CardToken { get; set; }

        /// <summary>
        /// Indicates the state of the card on the Marqeta network.
        /// </summary>
        [JsonPropertyName("state")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MarqetaCardState CardStatus { get; set; }

        /// <summary>
        /// Reason Code for changing the state of the card on Marqeta network.
        /// </summary>
        /// <remarks>
        /// Defaulted to (15: Initiated by issuer) Prog is issuer.
        /// </remarks>
        [JsonPropertyName("reason_code")]
        public string ReasonCode { get; set; } = "15";
    }
}
