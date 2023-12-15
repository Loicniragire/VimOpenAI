using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.DTO
{
    [ExcludeFromCodeCoverage]
    public class GooglePayTokenizationData
    {
        public string DisplayName { get; set; }
        public string LastDigits { get; set; }
        public string Network { get; set; }
        public string TokenServiceProvider { get; set; }
        public string OpaquePaymentCard { get; set; }
        public GooglePayUserAddress UserAddress { get; set; }
    }
}
