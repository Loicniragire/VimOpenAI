using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests.ValidationAttributes;
using VirtualPaymentService.Model.Types;

namespace VirtualPaymentService.Model.Requests
{
    [ExcludeFromCodeCoverage]
    public class VCardPurchaseAuthRequest
    {
        /// <summary>
        /// <see cref="bool"/> reflecting if the auth is a reversal.
        /// </summary>
        public bool IsReversal => TypeCode == Constants.Authorizations.ReversalType;

        /// <summary>
        /// The provider for the vcard used to make the transaction.
        /// </summary>
        [Required]
        public VirtualCardProviderNetwork? VCardProviderId { get; set; }

        /// <summary>
        /// The dollar amount of the authorization.
        /// </summary>
        [Required]
        public decimal? Amount { get; set; }

        /// <summary>
        /// The id assigned by the vcard provider to the vcard associated
        /// with the transaction.
        /// </summary>
        [Required]
        [MinLength(1)]
        public string ReferenceId { get; set; }

        /// <summary>
        /// The unique authorization identifier assigned to
        /// the transaction by the vcard provider.
        /// </summary>
        [Required]
        [MinLength(1)]
        public string AuthorizationId { get; set; }

        /// <summary>
        /// The DateTime at which the auth occurred.
        /// </summary>
        [Required]
        public DateTime? AuthorizationDateTime { get; set; }

        /// <summary>
        /// A code designating whether the auth is an authorization "0100" or a reversal "0400"
        /// </summary>
        [Required]
        [StringRange(AllowableValues = new[] { Constants.Authorizations.ReversalType, Constants.Authorizations.AuthorizationType })]
        public string TypeCode { get; set; }

        /// <summary>
        /// A brief description of the type. May be empty.
        /// </summary>
        public string TypeDesc { get; set; }

        /// <summary>
        /// A code related to the approval. May be empty.
        /// </summary>
        public string ApprovalCode { get; set; }

        /// <summary>
        /// Either "Approval" or "Decline"
        /// </summary>
        [Required]
        [StringRange(AllowableValues = new[] { Constants.Authorizations.Approval, Constants.Authorizations.Decline })]
        public string Response { get; set; }

        /// <summary>
        /// A code related to the decline. May be empty.
        /// </summary>
        public string DeclineReasonCode { get; set; }

        /// <summary>
        /// The reason why the provider declined the transaction. May be empty.
        /// </summary>
        public string DeclineReasonMessage { get; set; }

        /// <summary>
        /// The reason why our JIT gateway declined the transaction. May be empty.
        /// </summary>
        public string ProgressiveDeclineReasonMessage { get; set; }

        /// <summary>
        /// The unique identifier of the merchant.
        /// </summary>
        public string MerchantId { get; set; }

        /// <summary>
        /// The name of the merchant.
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// The city the merchant resides in.
        /// </summary>
        public string MerchantCity { get; set; }

        /// <summary>
        /// The state the merchant resides in.
        /// </summary>
        public string MerchantStateProvince { get; set; }

        /// <summary>
        /// The postal code the merchant resides in.
        /// </summary>
        public string MerchantPostalCode { get; set; }

        /// <summary>
        /// The country the merchant resides in.
        /// </summary>
        public string MerchantCountry { get; set; }

        /// <summary>
        /// The MCC the merchant falls under.
        /// </summary>
        public string MerchantCategoryCode { get; set; }

        /// <summary>
        /// The currency code of the location where the transaction occurred.
        /// </summary>
        public string SourceCurrencyCode { get; set; }
    }
}
