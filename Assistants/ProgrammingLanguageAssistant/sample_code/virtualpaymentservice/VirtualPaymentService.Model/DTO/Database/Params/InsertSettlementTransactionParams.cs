using System.Diagnostics.CodeAnalysis;
using static Dapper.SqlMapper;

namespace VirtualPaymentService.Model.DTO.Database.Params
{
    [ExcludeFromCodeCoverage]
    public class InsertSettlementTransactionParams
    {
        /// <summary>
        /// SQL user defined table type of Store.VPayReconciliation_TableType
        /// for input parameter @VPayReconTableType in Progressive database 
        /// for Store.VPayReconciliation_SaveList procedure.
        /// </summary>
        public ICustomQueryParameter VPayReconTableType { get; set; }
    }
}
