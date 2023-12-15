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
using VirtualPaymentService.Model.DTO.Database.Result;

namespace VirtualPaymentService.Data
{
    [ExcludeFromCodeCoverage]
    public class VCardPurchaseAuthRepository : Repository<VCardPurchaseAuth>, IVCardPurchaseAuthRepository
    {
        #region Constructor
        public VCardPurchaseAuthRepository(IDbConnectionFactory dbConnectionFactory)
            : base(Constants.ProgressiveDatabase, dbConnectionFactory) { }
        #endregion Constructor

        #region Method
        /// <inheritdoc />
        public async Task<InsertVCardPurchaseAuthResult> InsertVCardPurchaseAuthAsync(InsertVCardPurchaseAuthParams authParams)
        {
            var procedure = "VPay.VCardPurchaseAuth_ip";

            using var connection = GetConnection;
            return await connection.QuerySingleOrDefaultAsync<InsertVCardPurchaseAuthResult>(procedure, authParams, commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public async Task<IList<VCardPurchaseAuthResult>> GetVCardPurchaseAuthAsync(int vCardId)
        {
            var procedure = "vpay.VCardPurchaseAuth_Get_ByVCardId";
            var sqlParams = new
            {
                vCardId
            };

            using var connection = GetConnection;
            return (await connection.QueryAsync<VCardPurchaseAuthResult>(procedure, sqlParams, commandType: CommandType.StoredProcedure)).ToList();
        }
        #endregion Method
    }
}
