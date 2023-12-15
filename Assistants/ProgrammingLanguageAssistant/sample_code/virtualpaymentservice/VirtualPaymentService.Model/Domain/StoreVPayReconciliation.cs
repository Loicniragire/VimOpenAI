using Dapper.Contrib.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace VirtualPaymentService.Model.Domain
{
    [ExcludeFromCodeCoverage]
    [Table("Store.VPayReconciliation")]
    public class StoreVPayReconciliation
    {
        /// <summary>
        /// Internal identifier of the reconciliation record.
        /// </summary>
        public int VPayReconciliationId { get; set; }

        /// <summary>
        /// Internal identifier of the lease.
        /// </summary>
        public int LeaseId { get; set; }

        /// <summary>
        /// Unique identifier of the virtual card on the provider network.
        /// </summary>
        public string ProviderCardId { get; set; }

        /// <summary>
        /// Internal identifier for the store.
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Unique identifier of the reconciliation/settlement record on provider network.
        /// </summary>
        public string ProviderTransactionIdentifier { get; set; }

        /// <summary>
        /// Amount of the transaction.
        /// </summary>
        public decimal TransactionAmount { get; set; }

        /// <summary>
        /// Date of the transaction.
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// The transaction type, usually D for debit and C for credit.
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Date and time transaction posted.
        /// </summary>
        public DateTime PostedDate { get; set; }

        /// <summary>
        /// Date and time record was created in database.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// The internal identifier for a matching store transaction.
        /// If null then record has not been linked to a store transaction.
        /// </summary>
        public long StoreTransactionID { get; set; }
    }
}
