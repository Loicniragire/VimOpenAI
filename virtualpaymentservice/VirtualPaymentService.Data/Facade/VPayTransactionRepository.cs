using Dapper;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VirtualPaymentService.Data.Interface;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO.Database.Params;
using VirtualPaymentService.Model.DTO.Database.Result;
using VirtualPaymentService.Model.Extensions;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Data.Facade
{
    [ExcludeFromCodeCoverage]
    public class VPayTransactionRepository : IVPayTransactionRepository
    {
        private readonly IVCardRepository _vCardRepo;
        private readonly IVCardPurchaseAuthRepository _vCardAuthRepo;
        private readonly IStoreVPayReconciliationRepository _vCardReconRepo;

        public VPayTransactionRepository(IVCardRepository vCardRepo, IVCardPurchaseAuthRepository vCardAuthRepo, IStoreVPayReconciliationRepository vCardReconRepo)
        {
            _vCardRepo = vCardRepo;
            _vCardAuthRepo = vCardAuthRepo;
            _vCardReconRepo = vCardReconRepo;
        }

        /// <inheritdoc />
        public async Task<InsertVCardPurchaseAuthResult> InsertVCardPurchaseAuthAsync(InsertVCardPurchaseAuthParams authParams)
        {
            return await _vCardAuthRepo.InsertVCardPurchaseAuthAsync(authParams);
        }

        public async Task<IList<VCardPurchaseAuthResult>> GetVCardPurchaseAuthAsync(int vCardId)
        {
            return await _vCardAuthRepo.GetVCardPurchaseAuthAsync(vCardId);
        }

        /// <inheritdoc />
        public async Task<int> UpdateVCardAsync(VCardUpdateParams updateParams)
        {
            return await _vCardRepo.UpdateVCardAsync(updateParams);
        }

        /// <inheritdoc />
        public async Task<VCard> GetVCardByIdAsync(int vCardId)
        {
            return await _vCardRepo.GetVCardByIdAsync(vCardId);
        }

        /// <inheritdoc />
        public async Task InsertSettlementTransactionAsync(SettlementTransaction settlementTransaction)
        {
            // Convert the single transaction to a data table. The procedure supports multiple
            // transactions to be inserted but we are inserting one at a time to allow partial 
            // inserts to be supported.
            var settlementTransactions = new List<SettlementTransaction> { settlementTransaction }.ToDataTable();

            // Converting the list of transactions into specific type Dapper needs for passing to SQL.
            var settlementParams = new InsertSettlementTransactionParams { VPayReconTableType = settlementTransactions.AsTableValuedParameter("Store.VPayReconciliation_TableType") };

            await _vCardReconRepo.InsertSettlementTransactionAsync(settlementParams);
        }

        /// <inheritdoc />
        public async Task<IList<StoreVPayReconciliation>> GetSettlementTransactionAsync(int leaseId)
        {
            return await _vCardReconRepo.GetSettlementTransactionAsync(leaseId);
        }
    }
}
