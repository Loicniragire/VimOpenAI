using System.Collections.Generic;
using VirtualPaymentService.Model.Domain;

namespace VirtualPaymentService.Model.Responses
{
    /// <summary>
    /// The settlement transactions for a lease.
    /// </summary>
    public class GetSettlementTransactionsResponse
    {
        public IList<StoreVPayReconciliation> Settlements { set; get; } = new List<StoreVPayReconciliation>();
    }
}
