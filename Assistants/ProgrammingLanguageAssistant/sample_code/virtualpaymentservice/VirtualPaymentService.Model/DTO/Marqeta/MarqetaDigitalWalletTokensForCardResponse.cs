using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace VirtualPaymentService.Model.DTO.Marqeta
{
    /// <summary>
    /// Get digital wallet tokens for card response contract from Marqeta API https://www.marqeta.com/docs/core-api/digital-wallets-management#_list_digital_wallet_tokens_for_card
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaDigitalWalletTokensForCardResponse
    {
        /// <summary>
        /// Count of digital wallet tokens returned.
        /// </summary>
        /// <remarks>
        /// Zero is a valid value and notes no wallet tokens for the card.
        /// </remarks>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// The zero-n digital wallet tokens for the card.
        /// </summary>
        [JsonPropertyName("data")]
        public List<MarqetaDigitalWalletToken> Data { get; set; }
    }
}
