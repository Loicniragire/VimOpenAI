using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.DTO.Marqeta
{
    /// <summary>
    /// Contains information held by the card network and used for address verification of the cardholder. 
    /// This object is provided when address information was included as part of the tokenization request.
    /// Reference from Marqeta https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_wallet_provider_profile_object_response
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaDigitalWalletTokenProviderProfile
    {
        /// <summary>
        /// Contains information related to the cardholder, provided by the digital wallet provider. 
        /// This value is returned as provided by the digital wallet.
        /// </summary>
        [JsonPropertyName("account")]
        public MarqetaDigitalWalletTokenProviderProfileAccount Account { get; set; }

        /// <summary>
        /// Contains the digital wallet provider’s risk assessment for provisioning the digital wallet token. 
        /// This value is returned as provided by the digital wallet.
        /// </summary>
        [JsonPropertyName("risk_assessment")]
        public MarqetaDigitalWalletTokenProviderProfileRiskAssessment RiskAssessment { get; set; }

        /// <summary>
        /// The score from the device. This value is returned as provided by the digital wallet.
        /// </summary>
        [JsonPropertyName("device_score")]
        public string DeviceScore { get; set; }

        /// <summary>
        /// The source from which the digital wallet provider obtained the card PAN. This value is returned as provided by the digital wallet.
        /// </summary>
        /// <remarks>
        /// Allowable Values: KEY_ENTERED, ON_FILE, MOBILE_BANKING_APP
        /// </remarks>
        [JsonPropertyName("pan_source")]
        public string PanSource { get; set; }

        /// <summary>
        /// The source from which the digital wallet provider obtained the card PAN. This value is returned as provided by the digital wallet.
        /// </summary>
        /// <remarks>
        /// Allowable Values: Please see https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_wallet_provider_profile_object_response
        /// </remarks>
        [JsonPropertyName("reason_code")]
        public string ReasonCode { get; set; }
    }
}
