using System.Threading.Tasks;
using VirtualPaymentService.Data.Configuration;
using VirtualPaymentService.Model.Domain;

namespace VirtualPaymentService.Data.Interface
{
    public interface IVCardProviderProductTypeRepository
    {
        /// <summary>
        /// Inserts a new record for additional <see cref="VCard"/> information into the <see cref="VCardProviderProductType"/> 
        /// table in the <see cref="Constants.VirtualPaymentDatabase"/>
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <returns></returns>
        Task InsertVCardProviderProductTypeAsync(VCardProviderProductType cardInfo);

        /// <summary>
        /// Searches for the first <see cref="VCardProviderProductType"/> by referenceId
        /// </summary>
        /// <param name="referenceId">Card token to search record for.</param>
        /// <returns></returns>
        Task<VCardProviderProductType> GetVCardProviderProductTypeAsync(string referenceId);

    }
}
