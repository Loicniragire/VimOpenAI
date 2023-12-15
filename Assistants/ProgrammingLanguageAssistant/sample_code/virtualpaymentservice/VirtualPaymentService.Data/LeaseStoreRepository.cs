using Dapper;
using ProgLeasing.System.Data.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VirtualPaymentService.Data.Configuration;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;

namespace VirtualPaymentService.Data
{
    [Obsolete("This is a temporary solution for pulling lease/store data until VPO is able to provide it. Remove as soon as possible.")]
    [ExcludeFromCodeCoverage]
    public class LeaseStoreRepository : Repository<JITFundingValidation>, ILeaseStoreRepository
    {
        public LeaseStoreRepository(IDbConnectionFactory dbConnectionFactory)
            : base(Constants.ProgressiveDatabase, dbConnectionFactory) { }

        public JITFundingValidation GetDataForAuthorizationByLeaseId(int leaseId, string providerCardId)
        {
            using (var connection = GetConnection)
            {
                var procedure = "Progressive.VPay.VCard_Get_DataForAuthorization";
                var sqlParams = new
                {
                    LeaseId = leaseId,
                    ProviderCardId = providerCardId
                };

                return connection.QuerySingleOrDefault<JITFundingValidation>(procedure, sqlParams,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public string GetStateForValidationByMarqetaStateId(int marqetaStateId)
        {
            using (var connection = GetConnection)
            {
                var procedure = "Progressive.Vpay.MarqetaStateMapping_Get_ByStateID";
                var sqlParams = new
                {
                    StateId = marqetaStateId
                };

                return connection.ExecuteScalar<string>(procedure, sqlParams,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<StoreCustomField>> GetStoreCustomFields(int leaseId)
        {
            var procedure = "Progressive.Lease.GetInfoForVPay_sp";
            var sqlParams = new
            {
                LeaseId = leaseId
            };

            using var connection = GetConnection;
            return await connection.QueryAsync<StoreCustomField>(procedure, sqlParams, commandType: CommandType.StoredProcedure);
        }
    }
}
