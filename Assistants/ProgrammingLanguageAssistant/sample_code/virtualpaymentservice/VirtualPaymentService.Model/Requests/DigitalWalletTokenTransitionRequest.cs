using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Requests
{
    /// <summary>
    /// Request contract for digital wallet token transition
    /// References: https://www.marqeta.com/docs/core-api/digital-wallets-management#_sample_request_body_3
    /// </summary>
    [ExcludeFromCodeCoverage]

    public class DigitalWalletTokenTransitionRequest
    {
        /// <summary>
        /// Digital Wallet DTO containing card token and wallet specific token and properties.
        /// </summary>
        [Required]
        public DigitalWalletToken DigitalWalletToken { get; set; }

        /// <summary>
        /// Digital Wallet Token Status to transition associated wallet token to.
        /// </summary>
        [Required]
        public DigitalWalletTokenStatus WalletTokenTransitionStatus { get; set; }

        /// <summary>
        /// Digital Wallet Reason Code to supply to vendor, as to reason of token transition.
        /// </summary>
        [Required]
        public string WalletTransitionReasonCode { get; set; }
    }
}
