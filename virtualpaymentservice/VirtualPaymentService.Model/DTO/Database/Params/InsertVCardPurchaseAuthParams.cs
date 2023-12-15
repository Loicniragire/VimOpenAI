using System;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Model.DTO.Database.Params
{
    [ExcludeFromCodeCoverage]
    public class InsertVCardPurchaseAuthParams
    {
        public bool IsPush { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.VCardProviderId"/>
        public byte ProviderId { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.AuthorizationId"/>
        public string AuthorizationId { get; set; }

        public string CardNumber { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.AuthorizationDateTime"/>
        public DateTime AuthDateTime { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.Amount"/>
        public decimal Amount { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.ApprovalCode"/>
        public string ApprovalCode { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.Response"/>
        public string Response { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.TypeCode"/>
        public string TypeCode { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.TypeDesc"/>
        public string TypeDesc { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.DeclineReasonCode"/>
        public string DeclineReasonCode { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.DeclineReasonMessage"/>
        public string DeclineReasonMessage { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.ProgressiveDeclineReasonMessage"/>
        public string ProgressiveDeclineReasonMessage { get; set; }

        /// <summary>
        /// AvailableCredit isn't looked at or set at any point.
        /// </summary>
        public decimal AvailableCredit { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.MerchantCategoryCode"/>
        public string CategoryCode { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.MerchantId"/>
        public string MerchantId { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.MerchantName"/>
        public string MerchantName { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.MerchantCity"/>
        public string MerchantCity { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.MerchantStateProvince"/>
        public string MerchantStateProvince { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.MerchantPostalCode"/>
        public string MerchantPostalCode { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.MerchantCountry"/>
        public string MerchantCountry { get; set; }

        /// <summary>
        /// Currently is neither set or read, but is a parameter.
        /// </summary>
        public string MerchantAcquirer { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.SourceCurrencyCode"/>
        public string SourceCurrencyCode { get; set; }

        /// <summary>
        /// Currently is neither set or read, but is a parameter.
        /// </summary>
        public decimal SourceCurrencyAmount { get; set; }

        /// <summary>
        /// Currently is neither set or read, but is a parameter.
        /// </summary>
        public decimal CurrencyConversionRate { get; set; }

        /// <summary>
        /// Currently is neither set or read, but is a parameter.
        /// </summary>
        public string BillingCurrencyCode { get; set; }

        /// <inheritdoc cref="VCardPurchaseAuthRequest.ReferenceId"/>
        public string ReferenceId { get; set; }
    }
}
