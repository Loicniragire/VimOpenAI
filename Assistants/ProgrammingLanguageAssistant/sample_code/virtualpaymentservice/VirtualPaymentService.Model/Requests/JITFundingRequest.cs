using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Requests
{
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Reviewed")]
    [ExcludeFromCodeCoverage]
    public class JITFundingRequest
    {
        /// <summary>
        /// The unique ID of the lease from the Progressive systems.
        /// </summary>
        /// <remarks>
        /// The "user_token" from the Marqeta request. The LeaseId is set as the user identifier when we create the card at Marqeta.
        /// </remarks>
        [Required]
        public long? LeaseId { get; set; }

        /// <summary>
        /// The unique token used to identify a virtual card on the card provider network.
        /// </summary>
        /// <remarks>
        /// The "card_token" from the Marqeta request.
        /// Maps to the ReferenceId of the vpay.VCard table.
        /// </remarks>
        [Required]
        public string ProviderCardId { get; set; }

        /// <summary>
        /// Identifies the JIT funding request and response on the card provider network.
        /// </summary>
        /// <remarks>
        /// The "token" from the Marqeta request. 
        /// </remarks>
        [Required]
        public string TransactionToken { get; set; }

        /// <summary>
        /// The amount to authorize the JIT request for.
        /// </summary>
        /// <remarks>
        /// The "gpa_order.jit_funding.amount" from the Marqeta request.
        /// </remarks>
        [Required]
        public decimal TransactionAmount { get; set; }

        /// <summary>
        /// The date and time of the JIT transaction on the vcard provider network.
        /// </summary>
        /// <remarks>
        /// The "user_transaction_time" from the Marqeta request.
        /// </remarks>
        [Required]
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// The type of transaction being requested. Currently only the type AUTHORIZATION is supported for this endpoint.
        /// </summary>
        /// <remarks>
        /// The "type" from the Marqeta request.
        /// </remarks>
        [Required]
        public string TransactionType { get; set; }

        /// <summary>
        /// The 2 char state abbreviation where the card was charged from the provider network.
        /// This is required if the property <see cref="UseStateValidation"/> is set to true.
        /// </summary>
        /// <remarks>
        /// The "card_acceptor.state" from the Marqeta request.
        /// </remarks>
        public string TransactionState { get; set; }

        /// <summary>
        /// The status of the lease.
        /// </summary>
        [Required]
        public string LeaseStatus { get; set; }

        /// <summary>
        /// The two char abbreviation of the state the store is located in related to the lease.
        /// This is required if the property <see cref="UseStateValidation"/> is set to true.
        /// </summary>
        public string StoreAddressState { get; set; }

        /// <summary>
        /// The value of the store setting IsMinAmountRequired for the specific store related to the lease.
        /// </summary>
        [Required]
        public bool IsMinAmountRequired { get; set; }

        // The value of the setting for the store related to the lease.
        /// <summary>
        /// The value of the store setting UseStateValidation for the specific store related to the lease.
        /// </summary>
        [Required]
        public bool UseStateValidation { get; set; }
    }
}