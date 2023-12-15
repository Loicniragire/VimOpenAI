using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.Requests.Marqeta
{
    /// <summary>
    /// Marqeta POST card request https://www.marqeta.com/docs/core-api/cards#postCards
    /// </summary>
    public class MarqetaCardRequest
    {
        /// <summary>
        /// The identifier/token that we generate to uniquely identify this card.
        /// </summary>
        /// <remarks>
        /// Marqeta only supports a length of 1-36 for the token value.
        /// </remarks>
        [JsonPropertyName("token")]
        [Required]
        [MaxLength(36)]
        public string Token { get; set; }

        /// <summary>
        /// The identifier/token of the existing user on the Marqeta network.
        /// </summary>
        /// <remarks>
        /// The token for the user is the Progressive LeaseId.
        /// </remarks>
        [JsonPropertyName("user_token")]
        [Required]
        public string UserToken { get; set; }

        /// <summary>
        /// The unique identifier of the card product, configured on Progressive's Marqeta account,
        /// that we want to create.
        /// </summary>
        /// <remarks>
        /// The settings of the card product determine many of the rules of the card and how Marqeta will create it.
        /// </remarks>
        [JsonPropertyName("card_product_token")]
        [Required]
        public string CardProductToken { get; set; }
    }
}
