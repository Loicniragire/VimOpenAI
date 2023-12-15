using System.Threading.Tasks;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Business.Interface
{
    public interface IVPayTransactionProvider
    {
        /// <summary>
        /// Adds an authorization to the VCardPurchaseAuth table for the given vcard.
        /// </summary>
        /// <param name="request"><see cref="VCardPurchaseAuthRequest"/></param>
        /// <returns><see cref="AddVCardAuthorizationResult"/></returns>
        Task<AddVCardAuthorizationResult> AddVCardAuthorizationAsync(VCardPurchaseAuthRequest request);

        /// <summary>
        /// Gets saved authorizations for a virtual card.
        /// </summary>
        /// <param name="vCardId">Internal identifier of virtual card.</param>
        /// <returns><see cref="GetVirtualCardAuthorizationsResponse"/></returns>
        Task<GetVirtualCardAuthorizationsResponse> GetVirtualCardAuthorizationsAsync(int vCardId);

        /// <summary>
        /// Saves settlement transaction/s contained in the request.
        /// </summary>
        /// <param name="request"><see cref="SettlementTransactionRequest"/></param>
        /// <returns><see cref="Task"/></returns>
        Task AddSettlementTransactionAsync(SettlementTransactionRequest request);

        /// <summary>
        /// Gets saved settlement transactions for a lease.
        /// </summary>
        /// <param name="leaseId">The internal lease identifier.</param>
        /// <returns><see cref="GetSettlementTransactionsResponse"/></returns>
        Task<GetSettlementTransactionsResponse> GetSettlementTransactionsAsync(int leaseId);
    }
}
