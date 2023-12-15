using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.DTO.Marqeta;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.DTO
{
    /// <summary>
    /// Digital wallet token for a card, this is a minimal version of the data returned in <see cref="MarqetaDigitalWalletToken"/> from Marqeta.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DigitalWalletToken
    {
        /// <summary>
        /// The unique identifier of the digital wallet token.
        /// </summary>
        [Required]
        public string WalletToken { get; set; }

        /// <summary>
        /// State of the digital wallet token.
        /// </summary>
        [Required]
        public DigitalWalletTokenStatus WalletTokenStatus { get; set; }

        /// <summary>
        /// The type of device the token was created for.
        /// </summary>
        /// <remarks>
        /// The value returned may vary depending on the provider, this value is informational and is not actionable.
        /// </remarks>
        public string DeviceType { get; set; }

        /// <summary>
        /// The unique token of the card we searched against.
        /// </summary>
        [Required]
        public string CardToken { get; set; }
    }
}
