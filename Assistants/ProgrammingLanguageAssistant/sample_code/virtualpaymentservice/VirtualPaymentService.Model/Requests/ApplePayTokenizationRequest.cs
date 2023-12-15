using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.DTO;

namespace VirtualPaymentService.Model.Requests
{
    // Reference: https://www.marqeta.com/docs/core-api/digital-wallets-management#_sample_request_body
    [ExcludeFromCodeCoverage]
    public class ApplePayTokenizationRequest
    {
        [Required]
        public long? LeaseId { get; set; }

        [Required]
        public ApplePayProvisioningData Data { get; set; }
    }
}
