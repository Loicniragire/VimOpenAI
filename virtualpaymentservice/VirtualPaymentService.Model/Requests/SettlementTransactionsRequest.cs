using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VirtualPaymentService.Model.Requests.ValidationAttributes;

namespace VirtualPaymentService.Model.Requests
{
    /// <summary>
    /// Request contract for endpoint POST /vcard/transaction/settlement
    /// </summary>
    public class SettlementTransactionRequest
    {
        /// <summary>
        /// List of settlement transactions to save.
        /// </summary>
        [Required]
        public List<SettlementTransaction> SettlementTransactions { get; set; }
    }

    public class SettlementTransaction
    {
        /// <summary>
        /// ID of the lease.
        /// </summary>
        [Required]
        public int? LeaseId { get; set; }

        /// <summary>
        /// The unique card provider ID of the virtual card (also known as ReferenceId in Prog systems).
        /// </summary>
        [Required]
        [MinLength(1)]
        public string ProviderCardId { get; set; }

        /// <summary>
        /// The ID of the store related to the lease.
        /// </summary>
        [Required]
        public int? StoreId { get; set; }

        /// <summary>
        /// Unique transaction identifier from the card provider network. 
        /// </summary>
        [Required]
        [MinLength(1)]
        public string ProviderTransactionIdentifier { get; set; }

        /// <summary>
        /// Amount of the transaction.
        /// </summary>
        [Required]
        public decimal? TransactionAmount { get; set; }

        /// <summary>
        /// Posting date of the transaction.
        /// </summary>
        [Required]
        public DateTime PostedDate { get; set; }

        /// <summary>
        /// Date of the transaction.
        /// </summary>
        [Required]
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Type of transaction (D)ebit or (C)edit.
        /// </summary>
        [Required]
        [StringRange(AllowableValues = new[] { "C", "D" })]
        public string TransactionType { get; set; }
    }
}
