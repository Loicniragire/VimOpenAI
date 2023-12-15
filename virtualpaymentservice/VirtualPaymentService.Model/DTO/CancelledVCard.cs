using System;
using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.DTO
{
    [ExcludeFromCodeCoverage]
    public class CancelledVCard
    {
        public int VCardId { get; set; }

        public byte VCardProviderId { get; set; }

        public long LeaseId { get; set; }

        public string ReferenceId { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
