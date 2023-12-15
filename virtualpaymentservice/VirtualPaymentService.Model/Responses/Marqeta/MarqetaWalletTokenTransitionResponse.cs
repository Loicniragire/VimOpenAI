using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Responses.Marqeta
{
    /// <summary>
    /// Translation request contract for Marqeta' /digitalwallettokentransitions endpoint.
    /// </summary>
    [ExcludeFromCodeCoverage]

    public class MarqetaWalletTokenTransitionResponse : MarqetaHttpErrorResponse
    {
        /// <summary>
        /// Digital wallet transition token, provided by digital wallet vendor, or consumer can specify when issuing request.
        /// </summary>
        [JsonPropertyName("token")]
        public string WalletTransitionToken { get; set; }

        /// <summary>
        /// State of the digital wallet token after transition.
        /// </summary>
        [JsonPropertyName("state")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MarqetaDigitalWalletTokenStatus State { get; set; }

        /// <summary>
        /// Type of transition that occurred and state of wallet token after request submitted.
        /// </summary>
        /// <remarks>
        /// Expected Values: Please see https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_type_field_response
        /// </remarks>
        [JsonPropertyName("type")]
        public string WalletTransitionType { get; set; }

        /// <summary>
        /// The digital wallet token’s provisioning fulfillment status after receiving transition response.
        /// </summary>
        /// <remarks>
        /// Expected Values: DECISION_RED, DECISION_YELLOW, DECISION_GREEN, REJECTED, PROVISIONED
        /// Please see https://www.marqeta.com/docs/core-api/digital-wallets-management#_the_fulfillment_status_field_response
        /// </remarks>
        [JsonPropertyName("fulfillment_status")]
        public string FulfillmentStatus { get; set; }
    }
}
