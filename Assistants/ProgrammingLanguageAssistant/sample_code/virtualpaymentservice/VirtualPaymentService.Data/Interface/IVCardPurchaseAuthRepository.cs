using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Model.DTO.Database.Params;
using VirtualPaymentService.Model.DTO.Database.Result;

namespace VirtualPaymentService.Data.Interface
{
    public interface IVCardPurchaseAuthRepository
    {
        /// <summary>
        /// Attempts to insert a new record in the VCardPurchaseAuth table.
        /// </summary>
        /// <param name="authParams">The data to be provided as params for the store proc.</param>
        /// <returns><see cref="InsertVCardPurchaseAuthResult"/></returns>
        /// <remarks>
        /// Will throw an exception if a duplicate <see cref="InsertVCardPurchaseAuthParams.AuthorizationId"/> is provided.
        /// <br />
        /// Will throw an exception if no VCard is found.
        /// </remarks>
        Task<InsertVCardPurchaseAuthResult> InsertVCardPurchaseAuthAsync(InsertVCardPurchaseAuthParams authParams);

        /// <summary>
        /// Gets all purchase authorizations from vpay.VCardPurchaseAuth by virtual card.
        /// </summary>
        /// <param name="vCardId">The internal identifier of the virtual card.</param>
        Task<IList<VCardPurchaseAuthResult>> GetVCardPurchaseAuthAsync(int vCardId);
    }
}
