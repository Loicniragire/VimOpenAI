using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.DTO
{
    [ExcludeFromCodeCoverage]
    public class GooglePayUserAddress
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
    }
}
