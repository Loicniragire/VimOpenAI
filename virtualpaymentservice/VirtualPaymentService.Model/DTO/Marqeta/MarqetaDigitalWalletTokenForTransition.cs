using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VirtualPaymentService.Model.DTO.Marqeta
{
    /// <summary>
    /// Marqeta DTO object for Digital Wallet Transition request. Embedded "token" property for object "digital_wallet_token"
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MarqetaDigitalWalletTokenForTransition
    {

        /// <summary>
        /// The unique identifier of the digital wallet token.
        /// </summary>
        /// <remarks>
        /// Marqeta states 36 char is max size.
        /// </remarks>
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
