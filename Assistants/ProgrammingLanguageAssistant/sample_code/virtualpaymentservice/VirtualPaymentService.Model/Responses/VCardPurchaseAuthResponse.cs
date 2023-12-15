using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Responses
{
    [ExcludeFromCodeCoverage]
    public class VCardPurchaseAuthResponse
    {
        /// <summary>
        /// The id assigned by the vcard provider to the vcard associated
        /// with the transaction.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// The available balance of the authed vcard.
        /// </summary>
        public decimal AvailableBalance { get; set; }

        /// <summary>
        /// The total amount the vcard has been authed for.
        /// </summary>
        public decimal CardBalance { get; set; }

        /// <summary>
        /// The status of the vcard.
        /// </summary>
        public CardStatus VCardStatusId { get; set; }
    }
}
