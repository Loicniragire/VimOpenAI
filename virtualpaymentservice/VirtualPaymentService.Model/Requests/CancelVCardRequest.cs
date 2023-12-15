using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Requests
{
    [ExcludeFromCodeCoverage]
    public class CancelVCardRequest
    {
        /// <summary>
        /// Provider of vcard to cancel.
        /// </summary>
        [Required]
        [EnumDataType(typeof(VirtualCardProviderNetwork), ErrorMessage = "The card provider passed in is not supported.")]
        public VirtualCardProviderNetwork VCardProviderId { get; set; }

        /// <summary>
        /// The CardToken or ReferenceId from the vcard provider.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// The LeaseId the VCard belongs to.
        /// </summary>
        public int LeaseId { get; set; }

        /// <summary>
        /// The ActiveToDate of the VCard.
        /// </summary>
        public DateTime ActiveToDate { get; set; }
    }
}
