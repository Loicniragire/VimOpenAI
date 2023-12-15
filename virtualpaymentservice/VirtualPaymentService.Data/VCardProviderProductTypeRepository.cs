using Dapper.Contrib.Extensions;
using ProgLeasing.System.Data.Contract;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Dapper;
using VirtualPaymentService.Data.Configuration;
using VirtualPaymentService.Data.Interface;
using VirtualPaymentService.Model.Domain;

namespace VirtualPaymentService.Data
{
    [ExcludeFromCodeCoverage]
    public class VCardProviderProductTypeRepository : Repository<VCardProviderProductType>, IVCardProviderProductTypeRepository
    {
        #region Constructor
        public VCardProviderProductTypeRepository(IDbConnectionFactory dbConnectionFactory)
            : base(Constants.VirtualPaymentDatabase, dbConnectionFactory) { }
        #endregion Constructor

        #region Method
        /// <inheritdoc/>
        public async Task InsertVCardProviderProductTypeAsync(VCardProviderProductType cardInfo)
        {
            using var conn = GetConnection;
            await conn.InsertAsync(cardInfo);
        }

        /// <inheritdoc />
        public async Task<VCardProviderProductType> GetVCardProviderProductTypeAsync(string referenceId)
        {
            var queryString =
                $"SELECT * FROM [VCard].{nameof(VCardProviderProductType)} WHERE {nameof(VCardProviderProductType.ReferenceId)} = @{nameof(referenceId)}";
            using var conn = GetConnection;
            return await conn.QuerySingleOrDefaultAsync<VCardProviderProductType>(queryString, new { ReferenceId = referenceId });
        }

        #endregion Method
    }
}
