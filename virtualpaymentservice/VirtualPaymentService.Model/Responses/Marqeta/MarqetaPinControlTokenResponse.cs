using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Responses.Marqeta
{
    /// <summary>
    /// Response contract for generating a pin token https://www.marqeta.com/docs/core-api/pins#postPinsControltoken
    /// </summary>
    public class MarqetaPinControlTokenResponse
    {
        [JsonPropertyName("control_token")]
        [Required]
        [MaxLength(36)]
        public string ControlToken { get; set; }
    }
}
