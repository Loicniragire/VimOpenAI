using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.DTO.Marqeta
{
    /// <summary>
    /// Marqeta digital wallet token response contract https://www.marqeta.com/docs/core-api/digital-wallets-management#_response_body
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaDigitalWalletToken
    {
        /// <summary>
        /// The unique identifier of the digital wallet token.
        /// </summary>
        /// <remarks>
        /// Marqeta states 36 char is max size.
        /// </remarks>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// The token of the card we searched against.
        /// </summary>
        /// <remarks>
        /// Marqeta states 36 char is max size.
        /// </remarks>
        [JsonPropertyName("card_token")]
        public string CardToken { get; set; }

        /// <summary>
        /// State of the digital wallet token.
        /// </summary>
        [JsonPropertyName("state")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MarqetaDigitalWalletTokenStatus State { get; set; }

        /// <summary>
        /// The reason the digital wallet token transitioned to its current state.
        /// </summary>
        [JsonPropertyName("state_reason")]
        public string StateReason { get; set; }

        /// <summary>
        /// The digital wallet token’s provisioning status.
        /// </summary>
        /// <remarks>
        /// Allowable Values: DECISION_RED, DECISION_YELLOW, DECISION_GREEN, REJECTED, PROVISIONED
        /// </remarks>
        [JsonPropertyName("fulfillment_status")]
        public string FulfillmentStatus { get; set; }

        /// <summary>
        /// The Marqeta platform’s decision as to whether the digital wallet token should be provisioned.
        /// </summary>
        [JsonPropertyName("issuer_eligibility_decision")]
        public string IssuerEligibilityDecision { get; set; }

        /// <summary>
        /// The date and time when the digital wallet token object was created.
        /// </summary>
        [JsonPropertyName("created_time")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// The date and time when the digital wallet token object was last modified.
        /// </summary>
        [JsonPropertyName("last_modified_time")]
        public DateTime LastModifiedTime { get; set; }

        /// <summary>
        /// Contains information held and provided by the token service provider (card network). 
        /// This object is returned when relevant information is provided by the token service provider.
        /// </summary>
        [JsonPropertyName("token_service_provider")]
        public MarqetaDigitalWalletTokenServiceProvider TokenServiceProvider { get; set; }

        /// <summary>
        /// Contains information related to the device being provisioned. This object is returned when relevant information 
        /// is provided by the token service provider, typically for device-based digital wallets such as GooglePay and ApplePay only.
        /// </summary>
        [JsonPropertyName("device")]
        public MarqetaDigitalWalletTokenDevice Device { get; set; }

        /// <summary>
        /// Contains information held and provided by the digital wallet provider. This object is returned when relevant information is provided by the wallet provider.
        /// </summary>
        [JsonPropertyName("wallet_provider_profile")]
        public MarqetaDigitalWalletTokenProviderProfile WalletProviderProfile { get; set; }

        /// <summary>
        /// Contains information held by the card network and used for address verification of the cardholder. 
        /// This object is provided when address information was included as part of the tokenization request.
        /// </summary>
        [JsonPropertyName("address_verification")]
        public MarqetaDigitalWalletTokenAddressVerification AddressVerification { get; set; }
    }
}
