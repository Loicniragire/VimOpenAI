using System;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Model.Domain
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    [ExcludeFromCodeCoverage]
    public class JITDecisionLog
    {
        public long JITDecisionLogId { get; set; }
        public long LeaseId { get; set; }
        public string VCardReferenceId { get; set; }
        public string TransactionToken { get; set; }
        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public string TransactionState { get; set; }
        public bool Approved { get; set; }
        public JITFundingDeclineReason DeclineReason { get; set; }
        public DateTime DecisionDate { get; set; }
        public string LeaseStatus { get; set; }
        public bool IsMinAmountRequired { get; set; }
        public bool UseStateValidation { get; set; }
        public string StoreAddressState { get; set; }
        public decimal AvailableBalance { get; set; }
        public DateTime CardActiveToDate { get; set; }
        public CardStatus CardStatus { get; set; }
    }
}
