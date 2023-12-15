using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.DTO.Marqeta
{
    /// <summary>
    /// Contains information held by the card network and used for address verification of the cardholder. 
    /// This object is provided when address information was included as part of the tokenization request.
    /// Reference from Marqeta https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_address_verification_object_response
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaDigitalWalletTokenAddressVerification
    {
        /// <summary>
        /// The name of the cardholder. This value is returned as provided by the digital wallet provider.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The street address of the cardholder. This value is returned as provided by the digital wallet provider.
        /// </summary>
        [JsonPropertyName("street_address")]
        public string StreetAddress { get; set; }

        /// <summary>
        /// The postal code of the cardholder. This value is returned as provided by the digital wallet provider.
        /// </summary>
        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; }
    }
}
