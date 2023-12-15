using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Requests
{
    [ExcludeFromCodeCoverage]
    public class LeaseStoreDataRequest
    {
        // This should eventually be a long, but this code will hopefully be gone by then.
        public int LeaseId { get; set; }
        public string ProviderCardId { get; set; }
    }
}