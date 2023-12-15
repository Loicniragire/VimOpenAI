using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Data.Interface;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO.Database.Params;
using VirtualPaymentService.Model.DTO.Database.Result;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Data.Facade
{
    public interface IVPayTransactionRepository
    {
        /// <inheritdoc cref="IVCardPurchaseAuthRepository.InsertVCardPurchaseAuthAsync(InsertVCardPurchaseAuthParams)"/>
        Task<InsertVCardPurchaseAuthResult> InsertVCardPurchaseAuthAsync(InsertVCardPurchaseAuthParams authParams);

        /// <inheritdoc cref="IVCardPurchaseAuthRepository.GetVCardPurchaseAuthAsync(int)"/>
        Task<IList<VCardPurchaseAuthResult>> GetVCardPurchaseAuthAsync(int vCardId);

        /// <inheritdoc cref="IVCardRepository.UpdateVCardAsync(VCardUpdateParams)"/>
        Task<int> UpdateVCardAsync(VCardUpdateParams updateParams);

        /// <inheritdoc cref="IVCardRepository.GetVCardByIdAsync(int)"/>
        Task<VCard> GetVCardByIdAsync(int vCardId);

        /// <summary>
        /// Inserts <see cref="SettlementTransaction"/> into the Store.VPayReconciliation table.
        /// </summary>
        /// <param name="settlementTransactions">The <see cref="SettlementTransaction"/> to insert.</param>
        Task InsertSettlementTransactionAsync(SettlementTransaction settlementTransaction);

        /// <inheritdoc cref="IStoreVPayReconciliationRepository.GetSettlementTransactionAsync(int)"/>
        Task<IList<StoreVPayReconciliation>> GetSettlementTransactionAsync(int leaseId);
    }
}
