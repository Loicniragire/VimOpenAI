using Dapper;
using ProgLeasing.System.Data.Contract;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using VirtualPaymentService.Data.Configuration;
using VirtualPaymentService.Data.Interface;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO.Database.Params;

namespace VirtualPaymentService.Data
{
    [ExcludeFromCodeCoverage]
    public class StoreVPayReconciliationRepository : Repository<StoreVPayReconciliation>, IStoreVPayReconciliationRepository
    {
        #region Constructor
        public StoreVPayReconciliationRepository(IDbConnectionFactory dbConnectionFactory)
            : base(Constants.ProgressiveDatabase, dbConnectionFactory) { }
        #endregion Constructor

        #region Method
        /// <inheritdoc />
        public async Task InsertSettlementTransactionAsync(InsertSettlementTransactionParams settlementParams)
        {
            var procedure = "Store.VPayReconciliation_SaveList";

            using var connection = GetConnection;
            await connection.ExecuteAsync(procedure, settlementParams, commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public async Task<IList<StoreVPayReconciliation>> GetSettlementTransactionAsync(int leaseId)
        {
            var procedure = "Store.VpayReconciliation_Get_ByLeaseId";
            var sqlParams = new
            {
                leaseId
            };

            using var connection = GetConnection;
            return (await connection.QueryAsync<StoreVPayReconciliation>(procedure, sqlParams, commandType: CommandType.StoredProcedure)).ToList();
        }
        #endregion Method
    }
}
