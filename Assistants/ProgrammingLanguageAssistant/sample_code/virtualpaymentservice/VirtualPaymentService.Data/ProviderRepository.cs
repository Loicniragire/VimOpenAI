using Dapper.Contrib.Extensions;
using ProgLeasing.System.Data.Contract;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VirtualPaymentService.Data.Configuration;
using VirtualPaymentService.Data.Interface;
using VirtualPaymentService.Model.Domain;

namespace VirtualPaymentService.Data
{
    [ExcludeFromCodeCoverage]
    public class ProviderRepository : Repository<VCardProvider>, IProviderRepository
    {
        #region Constructor
        public ProviderRepository(IDbConnectionFactory dbConnectionFactory)
            : base(Constants.VirtualPaymentDatabase, dbConnectionFactory) { }
        #endregion Constructor

        #region Method
        /// <inheritdoc />
        public async Task<IEnumerable<VCardProvider>> GetVCardProvidersAsync()
        {
            using var conn = GetConnection;
            return await conn.GetAllAsync<VCardProvider>();
        }

        /// <inheritdoc />
        public async Task<StoreProductType> GetStoreProductTypeAsync(int storeId)
        {
            using var conn = GetConnection;
            return await conn.GetAsync<StoreProductType>(storeId);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProviderProductType>> GetProviderProductTypeAsync()
        {
            using var conn = GetConnection;
            return await conn.GetAllAsync<ProviderProductType>();
        }
        #endregion Method
    }
}
