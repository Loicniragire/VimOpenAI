using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Data.Interface
{
    public interface IProviderRepository
    {
        /// <summary>
        /// Retrieves providers from the VCard.Provider table.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="VCardProvider"/></returns>
        Task<IEnumerable<VCardProvider>> GetVCardProvidersAsync();

        /// <summary>
        /// Searches to locate possible mapping of store and card product.
        /// </summary>
        /// <param name="storeId">The store that may have a mapping.</param>
        /// <returns><see cref="StoreProductType"/></returns>
        Task<StoreProductType> GetStoreProductTypeAsync(int storeId);

        /// <summary>
        /// Return the <see cref="ProductType"/>/s <see cref="VirtualCardProviderNetwork"/>/s supports.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ProviderProductType"/></returns>
        Task<IEnumerable<ProviderProductType>> GetProviderProductTypeAsync();
    }
}
