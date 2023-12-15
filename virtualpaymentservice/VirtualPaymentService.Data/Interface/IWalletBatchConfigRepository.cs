using System.Threading.Tasks;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.Requests;

namespace VirtualPaymentService.Data.Interface
{
    public interface IWalletBatchConfigRepository
    {
        /// <summary>
        /// Asynchronously retrieves the TOP 1 record in the WalletBatchConfig table.
        /// </summary>
        /// <returns><see cref="WalletBatchConfig"/></returns>
        Task<WalletBatchConfig> GetWalletBatchConfigAsync();

        /// <summary>
        /// Updates the last process date in the WalletBatchConfig table.
        /// </summary>
        /// <param name="request">The update request body</param>
        /// <returns>A <see cref="Task"/></returns>
        Task UpdateWalletBatchConfigAsync(UpdateWalletBatchConfigRequest request);
    }
}
