using Dapper;
using ProgLeasing.System.Data.Contract;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VirtualPaymentService.Data.Configuration;
using VirtualPaymentService.Data.Interface;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Data
{
    [ExcludeFromCodeCoverage]
    public class WalletBatchConfigRepository : Repository<WalletBatchConfig>, IWalletBatchConfigRepository
    {
        #region Constructor
        public WalletBatchConfigRepository(IDbConnectionFactory dbConnectionFactory)
            : base(Constants.VirtualPaymentDatabase, dbConnectionFactory) { }
        #endregion Constructor

        #region Method
        /// <inheritdoc />
        public async Task<WalletBatchConfig> GetWalletBatchConfigAsync()
        {
            using var conn = GetConnection;
            var procedure = "VCard.WalletBatchConfig_Get";

            return await conn.QuerySingleOrDefaultAsync<WalletBatchConfig>(procedure, commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public async Task UpdateWalletBatchConfigAsync(UpdateWalletBatchConfigRequest request)
        {
            var procedure = "VCard.WalletBatchConfig_Update";

            using var conn = GetConnection;
            await conn.QueryAsync(procedure, param: request, commandType: CommandType.StoredProcedure);
        }
        #endregion Method
    }
}
