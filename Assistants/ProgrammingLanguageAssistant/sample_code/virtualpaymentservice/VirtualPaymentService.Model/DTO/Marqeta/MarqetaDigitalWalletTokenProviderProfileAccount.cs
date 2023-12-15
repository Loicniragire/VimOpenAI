using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.DTO.Marqeta
{
    /// <summary>
    /// Contains information related to the cardholder, provided by the digital wallet provider. 
    /// This value is returned as provided by the digital wallet.
    /// Reference from Marqeta https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_wallet_provider_profile_account_object_response
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaDigitalWalletTokenProviderProfileAccount
    {
        /// <summary>
        /// The wallet provider’s unique identity number for the cardholder. This identity number does not correlate with other information.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The score from the digital wallet provider. This value is returned as provided by the digital wallet provider.
        /// </summary>
        [JsonPropertyName("score")]
        public string Score { get; set; }
    }
}
