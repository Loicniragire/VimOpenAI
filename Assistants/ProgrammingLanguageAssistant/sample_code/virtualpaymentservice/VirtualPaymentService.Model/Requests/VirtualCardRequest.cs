using System.ComponentModel.DataAnnotations;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Requests
{
    /// <summary>
    /// Request contract for endpoint POST /vcard
    /// </summary>
    public class VirtualCardRequest
    {
        /// <summary>
        /// ID of the lease.
        /// </summary>
        [Required]
        public int? LeaseId { get; set; }

        /// <summary>
        /// Provider to create the card with.
        /// </summary>
        [Required]
        [EnumDataType(typeof(VirtualCardProviderNetwork), ErrorMessage = "The card provider passed in is not supported.")]
        public VirtualCardProviderNetwork VCardProviderId { get; set; }

        /// <summary>
        /// The ID of the store the lease is related to.
        /// </summary>
        [Required]
        public int? StoreId { get; set; }

        /// <summary>
        /// The amount the card should be created for without any buffer for <see cref="MaxAmountGreater"/>.
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "The value must be greater than zero.")]
        public decimal OriginalCardBaseAmount { get; set; }

        /// <summary>
        /// The total amount the card should be created for. Typically calculated as <see cref="OriginalCardBaseAmount"/> + <see cref="MaxAmountGreater"/>.
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "The value must be greater than zero.")]
        public decimal AvailableBalance { get; set; }

        /// <summary>
        /// The balance of the card, typically set to zero (0.00).
        /// </summary>
        [Required]
        [Range(0.00, double.MaxValue, ErrorMessage = "The value must be zero or greater.")]
        public decimal? CardBalance { get; set; }

        /// <summary>
        /// Based on store setting allowing only one auth, the amount below the <see cref="AvailableBalance"/> to approve an authorization for.
        /// </summary>
        [Required]
        [Range(0.00, double.MaxValue, ErrorMessage = "The value must be zero or greater.")]
        public decimal? MaxAmountLess { get; set; }

        /// <summary>
        /// The buffer amount added to the <see cref="AvailableBalance"/>. 
        /// </summary>
        [Required]
        [Range(0.00, double.MaxValue, ErrorMessage = "The value must be zero or greater.")]
        public decimal? MaxAmountGreater { get; set; }

        /// <summary>
        /// The number of days, after the creation of the card, to allow authorizations to be approved. 
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be greater than zero.")]
        public int DaysActiveTo { get; set; }

        /// <summary>
        /// Optional value for setting the time of the active to date to include an additional number of minutes.
        /// The default value, if property not passed in, is 90 minutes.
        /// </summary>
        public int ActiveToBufferMinutes { get; set; } = 90;

        /// <summary>
        /// The customer information, if provided, to create a user at the card provider.
        /// </summary>
        public CardUserUpdateRequest User { get; set; }
    }
}
