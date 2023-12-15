using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Responses
{
    // Reference: https://www.marqeta.com/docs/core-api/digital-wallets-management#_sample_response_body
    [ExcludeFromCodeCoverage]
    public class ApplePayTokenizationResponse
    {
        public string CardToken { get; set; }
        public string EncryptedPassData { get; set; }
        public string ActivationData { get; set; }
        public string EphemeralPublicKey { get; set; }
    }
}
