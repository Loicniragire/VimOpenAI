using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO.Database.Params;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Data.Interface
{
    public interface IStoreVPayReconciliationRepository
    {
        /// <summary>
        /// Inserts settlement transactions into the Store.VPayReconciliation table.
        /// </summary>
        /// <param name="settlementParams">The <see cref="SettlementTransaction"/>s to insert.</param>
        Task InsertSettlementTransactionAsync(InsertSettlementTransactionParams settlementParams);

        /// <summary>
        /// Searches for saved settlement transactions for a lease.
        /// </summary>
        /// <param name="leaseId">The internal identifier for a lease.</param>
        Task<IList<StoreVPayReconciliation>> GetSettlementTransactionAsync(int leaseId);
    }
}
