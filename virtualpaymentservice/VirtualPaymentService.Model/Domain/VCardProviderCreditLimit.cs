using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Domain
{
    [ExcludeFromCodeCoverage]
    public  class VCardProviderCreditLimit
    {
        public decimal TotalAuthAmount { get; set; }
    }
}
