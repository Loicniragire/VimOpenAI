using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Domain
{
    [ExcludeFromCodeCoverage]
    public class VCard
    {
        public int VCardId { get; set; }
        public byte VCardProviderId { get; set; }

        /// <summary>
        /// The provider <see cref="ProductType"/> this card was created as.
        /// Only present when endpoint supports returning value currently (POST /vcard) and (GET /vcard/{leaseId}).
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ProductType? ProductTypeId { get; set; } = null;

        public long LeaseId { get; set; }
        public decimal CardBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public string CardNumber { get; set; }
        public string PinNumber { get; set; }
        public DateTime ActiveToDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ReferenceId { get; set; }
        public CardStatus VCardStatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public decimal OriginalCardBaseAmount { get; set; }
        public decimal MaxAmountGreater { get; set; }
        public decimal MaxAmountLess { get; set; }
    }
}
