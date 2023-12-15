using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.DTO.Database.Params
{
    [ExcludeFromCodeCoverage]
    public class VCardUpdateParams
    {
        public int VCardId { get; set; }

        public string ReferenceId { get; set; }

        public long LeaseId { get; set; }

        public CardStatus VCardStatusId { get; set; }

        public decimal CardBalance { get; set; }

        public decimal AvailableBalance { get; set; }
    }
}
