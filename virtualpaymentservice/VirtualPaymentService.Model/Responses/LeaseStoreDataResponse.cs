using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Responses
{
    [ExcludeFromCodeCoverage]
    public class LeaseStoreDataResponse : Response
    {
        public decimal InvoiceAmount { get; set; }
        public bool IsMinAmountRequired { get; set; }
        public string LeaseStatus { get; set; }
        public string State { get; set; }
        public bool UseStateValidation { get; set; }
    }
}