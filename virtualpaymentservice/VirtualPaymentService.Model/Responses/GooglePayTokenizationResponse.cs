using System;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.DTO;

namespace VirtualPaymentService.Model.Responses
{
    // Reference: https://www.marqeta.com/docs/core-api/digital-wallets-management#_sample_response_body_2
    [ExcludeFromCodeCoverage]
    public class GooglePayTokenizationResponse
    {
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string CardToken { get; set; }
        public GooglePayTokenizationData PushTokenizeRequestData { get; set; }
    }
}
