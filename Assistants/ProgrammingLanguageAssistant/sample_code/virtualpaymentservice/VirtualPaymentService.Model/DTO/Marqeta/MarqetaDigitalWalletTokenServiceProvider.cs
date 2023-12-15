using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.DTO.Marqeta
{
    /// <summary>
    /// Contains information held and provided by the token service provider (card network). 
    /// This object is returned when relevant information is provided by the token service provider.
    /// Reference from Marqeta https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_token_service_provider_object_response
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaDigitalWalletTokenServiceProvider
    {
        /// <summary>
        /// The unique identifier of the digital wallet token within the card network. The token_reference_id is unique at the card network level.
        /// </summary>
        [JsonPropertyName("token_reference_id")]
        public string TokenReferenceId { get; set; }

        /// <summary>
        /// The unique identifier of the digital wallet token PAN within the card network. This value may vary on a per-digital wallet basis. For example, 
        /// the pan_reference_id may be different in an Apple Pay wallet and a Google Pay wallet for the same digital wallet token.
        /// </summary>
        [JsonPropertyName("pan_reference_id")]
        public string PanReferenceId { get; set; }

        /// <summary>
        /// The unique numerical identifier of the token requestor within the card network. The token_requestor_id is unique at the card network level. 
        /// These ID numbers map to token_requestor_name field values can be found here: https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_token_service_provider_object_response
        /// </summary>
        [JsonPropertyName("token_requestor_id")]
        public string TokenRequestorId { get; set; }

        /// <summary>
        /// The name of the token requestor within the card network.
        /// </summary>
        [JsonPropertyName("token_requestor_name")]
        public string TokenRequestorName { get; set; }

        /// <summary>
        /// The type of digital wallet token.
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// The digital wallet token primary account number (PAN), also known as Digital Account Number, Token Number or Digital PAN (DPAN). 
        /// These are unique only at a given point in time and are subject to re-use. This value is provided after a token is activated.
        /// </summary>
        /// <remarks>
        /// Not allowing the property to be serialized/deserialized so it is not accidentally logged or sent in a response.
        /// This is here for documenting the contract with Marqeta and could be modified in the future if we need this value.
        /// </remarks>
        [JsonIgnore]
        [JsonPropertyName("token_pan")]
        public string TokenPan { get; set; }

        /// <summary>
        /// The expiration date of the digital wallet token. This value is provided after a token is activated.
        /// </summary>
        /// <remarks>
        /// In format MMYY.
        /// </remarks>
        [JsonPropertyName("token_expiration")]
        public string TokenExpiration { get; set; }

        /// <summary>
        /// The token score assigned by the token service provider. This value is provided when it is received by the token service provider.
        /// </summary>
        [JsonPropertyName("token_score")]
        public string TokenScore { get; set; }

        /// <summary>
        /// The token service provider’s (typically the network’s) recommendation as to whether the digital wallet token should be provisioned.
        /// </summary>
        [JsonPropertyName("token_eligibility_decision")]
        public string TokenEligibilityDecision { get; set; }
    }
}
