using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.DTO.Marqeta
{
    /// <summary>
    /// Contains the digital wallet provider’s risk assessment for provisioning the digital wallet token. 
    /// This value is returned as provided by the digital wallet.
    /// Reference from Marqeta https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_wallet_provider_profile_risk_assessment_object_response
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaDigitalWalletTokenProviderProfileRiskAssessment
    {
        /// <summary>
        /// The wallet provider’s decision as to whether the digital wallet token should be provisioned.
        /// </summary>
        /// <remarks>
        /// Allowable Values: DECISION_RED, DECISION_YELLOW, DECISION_GREEN
        /// </remarks>
        [JsonPropertyName("score")]
        public string Score { get; set; }
    }
}
