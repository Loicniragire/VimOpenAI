using System.Threading.Tasks;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Business.Interface
{
    /// <summary>
    /// Interface that all VCardProviders should inherit from
    /// </summary>
    public interface ICardProvider
    {
        /// <summary>
        /// Readonly name of the provider (E.g. "Marqeta").
        /// </summary>
        public VPaymentProvider Provider { get; }

        /// <summary>
        /// Method which pings the provider. 
        /// This method will be used to determine whether the service state is healthy or unhealthy.
        /// </summary>
        /// <returns><see cref="bool"/>. If false the service will be unhealthy.</returns>
        public Task<bool> IsHealthyAsync();

        /// <summary>
        /// Creates a new virtual card on the provider network.
        /// </summary>
        /// <returns><see cref="VirtualCardResponse"/></returns>
        public Task<VirtualCardResponse> CreateCardAsync(VirtualCardRequest request, ProductType productType);

        /// <summary>
        /// Cancels the given vcard on the provider network.
        /// </summary>
        /// <param name="vCard">The vCard to cancel.</param>
        /// <param name="productType">Product type for which the virtual card was created with.</param>
        /// <returns><see cref="bool"/> designating if the card was cancelled successfully.</returns>
        public Task<bool> CancelCardAsync(CancelVCardRequest vCard, ProductType productType);

        /// <summary>
        /// Gets the card user properties set on the provider network.
        /// </summary>
        /// <param name="userToken">Unique user identifier on provider network.</param>
        /// <param name="productType">Product type of the card related to the user.</param>
        /// <returns><see cref="CardUserResponse"/></returns>
        Task<CardUserResponse> GetCardUserAsync(string userToken, ProductType productType);
    }
}
