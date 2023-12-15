using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.DTO;

namespace VirtualPaymentService.Model.Requests
{
    // Reference: https://www.marqeta.com/docs/core-api/digital-wallets-management#_sample_request_body_2
    [ExcludeFromCodeCoverage]
    public class GooglePayTokenizationRequest
    {
        [Required]
        public long? LeaseId { get; set; }

        [Required]
        public GooglePayProvisioningData Data { get; set; }
    }
}
