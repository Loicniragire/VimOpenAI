using Dapper.Contrib.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Domain
{
    [ExcludeFromCodeCoverage]
    [Table("VCardPurchaseAuth")]
    public class VCardPurchaseAuth
    {
        [Key]
        public int VCardPurchaseAuthId { get; set; }

        public int VCardHistoryId { get; set; }

        public string ProviderAuthorizationId { get; set; }

        public DateTime AuthorizationDateTime { get; set; }

        public decimal Amount { get; set; }

        public string ApprovalCode { get; set; }

        public string Response { get; set; }

        public string TypeCode { get; set; }

        public string TypeDesc { get; set; }

        public string DeclineReasonCode { get; set; }

        public string DeclineReasonMessage { get; set; }

        public decimal AvailableCredit { get; set; }

        public string CategoryCode { get; set; }

        public string MerchantId { get; set; }

        public string MerchantName { get; set; }

        public string MerchantCity { get; set; }

        public string MerchantStateProvince { get; set; }

        public string MerchantPostalCode { get; set; }

        public string MerchantCountry { get; set; }

        public string MerchantAcquirer { get; set; }

        public string SourceCurrencyCode { get; set; }

        public decimal SourceCurrencyAmount { get; set; }

        public decimal CurrencyConversionRate { get; set; }

        public string BillingCurrencyCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsPush { get; set; }

        public string ProgressiveDeclineReasonMessage { get; set; }
    }
}
