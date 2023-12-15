using System;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Domain
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    [ExcludeFromCodeCoverage]
    public class JITFundingValidation
    {
        public DateTime ActiveToDate { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal InvoiceAmount { get; set; }
        public bool IsMinAmountRequired { get; set; }
        public CardStatus VCardStatusId { get; set; }
        public string LeaseStatus { get; set; }
        public string State { get; set; }
        public bool UseStateValidation { get; set; }
        public string MarqetaState { get; set; }
    }
}
