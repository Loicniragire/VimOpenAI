using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VirtualPaymentService.Model.DTO.Marqeta;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Requests.Marqeta
{
    /// <summary>
    /// Marqeta request translation object. Could also specify property "token" if desired, to assign our own digital wallet transition token.
    /// </summary>
    [ExcludeFromCodeCoverage]

    public class MarqetaWalletTokenTransitionRequest
    {
        /// <summary>
        /// Digital wallet token to transition. Object to embed the digital wallet token, "token" property.
        /// </summary>
        [JsonPropertyName("digital_wallet_token")] 
        public MarqetaDigitalWalletTokenForTransition DigitalWalletToken { get; set; }

        /// <summary>
        /// State of the digital wallet token.
        /// </summary>
        [JsonPropertyName("state")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MarqetaDigitalWalletTokenStatus State { get; set; }

        /// <summary>
        /// The source for which the digital wallet provider requires a reason code for the transitioning of a digital wallet token.
        /// </summary>
        /// <remarks>
        /// Allowable Values: Please see https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_reason_code_field
        /// </remarks>
        [JsonPropertyName("reason_code")]
        public string ReasonCode { get; set; }
    }
}
