using Dapper;
using ProgLeasing.System.Data.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using VirtualPaymentService.Data.Configuration;
using VirtualPaymentService.Data.DynamicQueries;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.DTO.Database.Params;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Data
{
    [ExcludeFromCodeCoverage]
    public class VCardRepository : Repository<VCard>, IVCardRepository
    {
        #region Constructor
        public VCardRepository(IDbConnectionFactory dbConnectionFactory)
            : base(Constants.ProgressiveDatabase, dbConnectionFactory) { }
        #endregion Constructor

        #region Method
        /// <inheritdoc />
        public async Task<int> CancelVCardAsync(CancelVCardRequest card)
        {
            var procedure = "VPay.VCardCancel_ip";
            var sqlParams = new
            {
                card.LeaseId,
                card.ReferenceId,
                card.ActiveToDate,
            };

            using var connection = GetConnection;
            return await connection.ExecuteAsync(procedure, sqlParams, commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc/>
        public async Task<VCard> GetVCardAsync(long leaseId, string providerCardId)
        {
            var procedure = "VPay.VCard_Get_ByLeaseIdProviderCardId";
            var sqlParams = new
            {
                LeaseId = leaseId,
                ProviderCardId = providerCardId
            };

            using var connection = GetConnection;

            return await connection.QueryFirstOrDefaultAsync<VCard>(procedure, sqlParams, commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<VCard>> GetVCardsByLeaseAsync(long leaseId)
        {
            var procedure = "VPay.VCardByLease_sp";
            var sqlParams = new
            {
                LeaseId = leaseId,
                GetAll = 1
            };

            using var connection = GetConnection;
            return await connection.QueryAsync<VCard>(procedure, sqlParams, commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public async Task<IList<CancelledVCard>> GetCancelledVCardsByUpdatedDateAsync(DateTime localDateTime)
        {
            var procedure = "VPay.VCardCancel_Get_ByUpdatedDate";
            var sqlParams = new
            {
                UpdatedOnOrAfter = localDateTime
            };

            using var connection = GetConnection;
            return (await connection.QueryAsync<CancelledVCard>(procedure, sqlParams, commandType: CommandType.StoredProcedure)).ToList();
        }

        /// <inheritdoc />
        public async Task<int> InsertVCard(VCard card)
        {
            var procedure = "VPay.VCard_ip";
            var sqlParams = new
            {
                VCardProviderId = card.VCardProviderId,
                LeaseId = card.LeaseId,
                CardBalance = card.CardBalance,
                AvailableBalance = card.AvailableBalance,
                ReferenceId = card.ReferenceId,
                CardNumber = card.CardNumber,
                PinNumber = card.PinNumber,
                ActiveToDate = card.ActiveToDate,
                ExpirationDate = card.ExpirationDate,
                VCardStatusId = card.VCardStatusId,
                OriginalCardBaseAmount = card.OriginalCardBaseAmount,
                MaxAmountLess = card.MaxAmountLess,
                MaxAmountGreater = card.MaxAmountGreater,
                // This is hard coded to 1 since all cards created in VPS use the new flow.
                // Once the old VPayService is deprecated we can remove this flag from the
                // proc and the database table.
                IsNewCalcFlow = 1
            };

            using var connection = GetConnection;
            return await connection.ExecuteScalarAsync<int>(procedure, sqlParams, commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public async Task<VCardProviderCreditLimit> GetAuthTotalByProviderIdAsync(int vcardProviderId, DateTime startDate)
        {
            var procedure = "VPay.VCardProvider_Get_CreditLimit";
            var sqlParams = new
            {
                VCardProviderId = vcardProviderId,
                StartDate = startDate,
                EndDate = DateTime.Now
            };

            using var connection = GetConnection;

            return await connection.QuerySingleOrDefaultAsync<VCardProviderCreditLimit>(procedure, sqlParams, commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public async Task<VCard> GetVCardByIdAsync(int vCardId)
        {
            var procedure = "VPay.VCard_Get";
            var sqlParams = new
            {
                VCardId = vCardId
            };

            using var connection = GetConnection;
            return await connection.QuerySingleOrDefaultAsync<VCard>(procedure, sqlParams, commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public async Task<int> UpdateVCardAsync(VCardUpdateParams sqlParams)
        {
            var procedure = "VPay.VCard_up";

            using var connection = GetConnection;
            return await connection.ExecuteAsync(procedure, sqlParams, commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public async Task<IList<VCard>> GetFilteredVCardsAsync(GetVCardsDynamicQuery queryBuilder)
        {
            var parameterizedSql = queryBuilder.BuildParameterizedQuery($"SELECT TOP (@{nameof(queryBuilder.Limit)}) * FROM VPay.VCard");
            parameterizedSql.QueryParams.Add(nameof(queryBuilder.Limit), queryBuilder.Limit);

            using var conn = GetConnection;
            return (await conn.QueryAsync<VCard>(parameterizedSql.RawSql, parameterizedSql.QueryParams)).ToList();
        }
        #endregion Method
    }
}
