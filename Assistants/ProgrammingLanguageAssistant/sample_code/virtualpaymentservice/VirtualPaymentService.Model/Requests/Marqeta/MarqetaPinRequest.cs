using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Requests.Marqeta
{
    /// <summary>
    /// Request contract for setting or updating a pin number for a card https://www.marqeta.com/docs/core-api/pins#putPins
    /// </summary>
    public class MarqetaPinRequest
    {
        /// <summary>
        /// The unique card pin control token that allows access to set/create pin.
        /// </summary>
        /// <remarks>
        /// <see cref="ControlToken"/> is generated using https://www.marqeta.com/docs/core-api/pins#postPinsControltoken
        /// </remarks>
        [JsonPropertyName("control_token")]
        [Required]
        [MaxLength(36)]
        public string ControlToken { get; set; }

        /// <summary>
        /// Pin number to set or update on a card, must be 4 digits.
        /// </summary>
        [JsonPropertyName("pin")]
        [Required]
        [StringLength(4, ErrorMessage = "PIN must be 4 digits.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "PIN must only contain numeric values.")]
        public string Pin { get; set; }
    }
}
