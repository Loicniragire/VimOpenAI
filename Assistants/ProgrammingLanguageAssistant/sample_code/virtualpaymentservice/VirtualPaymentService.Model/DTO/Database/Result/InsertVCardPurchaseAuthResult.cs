using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Domain;

namespace VirtualPaymentService.Model.DTO.Database.Result
{
    [ExcludeFromCodeCoverage]
    public class InsertVCardPurchaseAuthResult
    {
        /// <summary>
        /// The primary id of the vcard the authorization was associated with.
        /// </summary>
        public int VCardId { get; set; }

        /// <summary>
        /// The primary id of the new entry within the VCardHistory table.
        /// </summary>
        public int VCardHistoryId { get; set; }

        /// <summary>
        /// The primary id of the new entry within the <see cref="VCardPurchaseAuth"/> table.
        /// </summary>
        public int VCardPurchaseAuthId { get; set; }

        /// <summary>
        /// The lease id with which the authorization was associated.
        /// </summary>
        public long LeaseId { get; set; }
    }
}
