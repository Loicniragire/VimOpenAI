using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Requests.Marqeta
{
    /// <summary>
    /// Request contract for generating a pin token https://www.marqeta.com/docs/core-api/pins#postPinsControltoken
    /// </summary>
    public class MarqetaPinControlTokenRequest
    {
        /// <summary>
        /// The unique card token on the marqeta network.
        /// </summary>
        [JsonPropertyName("card_token")]
        [Required]
        [MaxLength(36)]
        public string CardToken { get; set; }

        /// <summary>
        /// The type of token to generate (SET_PIN or REVEAL_PIN)
        /// </summary>
        [JsonPropertyName("controltoken_type")]
        [Required]
        public string ControlTokenType { get; set; }
    }
}
