using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.DTO;

namespace VirtualPaymentService.Model.Responses
{
    [ExcludeFromCodeCoverage]
    public class DigitalWalletTokenResponse
    {
        /// <summary>
        /// List of 0-n number of digital wallet tokens for a card.
        /// </summary>
        public List<DigitalWalletToken> DigitalWalletTokens { get; set; }
    }
}
