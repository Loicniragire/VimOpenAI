using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtualPaymentService.Data.Facade;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Business.Interface
{
    /// <summary>
    /// Provider that interacts with <see cref="ICardProvider"/>s.
    /// </summary>
    public interface IVirtualCardProvider
    {
        /// <summary>
        /// Method which gets necessary ApplePay tokenization data to add a card to Apple's Wallet, using an <see cref="IDigitalWalletProvider"/>.
        /// </summary>
        /// <param name="request">Base request which will be formatted for the specific <see cref="IDigitalWalletProvider"/> being used.</param>
        /// <returns><see cref="ApplePayTokenizationResponse"/></returns>
        Task<ApplePayTokenizationResponse> GetApplePayTokenizationDataAsync(ApplePayTokenizationRequest request);

        /// <summary>
        /// Method which gets necessary Google Pay tokenization data to add a card to Google's Wallet, using an <see cref="IDigitalWalletProvider"/>.
        /// </summary>
        /// <param name="request">Base request which will be formatted for the specific <see cref="IDigitalWalletProvider"/> being used.</param>
        /// <returns><see cref="GooglePayTokenizationResponse"/></returns>
        Task<GooglePayTokenizationResponse> GetGooglePayTokenizationDataAsync(GooglePayTokenizationRequest request);

        /// <summary>
        /// Returns all digital wallet tokens related to the virtual card.
        /// </summary>
        /// <param name="cardToken">The card token</param>
        /// <returns><see cref="DigitalWalletTokenResponse"/></returns>
        Task<DigitalWalletTokenResponse> GetDigitalWalletTokensByVCardAsync(string cardToken);

        /// <summary>
        /// Gets a list of VCards using the provided params to filter the results.
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns>A <see cref="GetVCardsResponse"/></returns>
        Task<GetVCardsResponse> GetVCardsByFilterAsync(GetVCardsRequest queryParams);

        /// <summary>
        /// Transitions digital wallet token to the transition status supplied and returns the digital wallet transition token.
        /// </summary>
        /// <param name="request">Contains the wallet token, new transition status, and reason code required for request to wallet provider</param>
        /// <returns><see cref="DigitalWalletTokenTransitionResponse"/></returns>
        Task<DigitalWalletTokenTransitionResponse> TransitionDigitalWalletTokenAsync(DigitalWalletTokenTransitionRequest request);

        /// <summary>
        /// Returns all VCards for a given LeaseId.
        /// </summary>
        /// <param name="leaseId"></param>
        /// <returns><see cref="IEnumerable{}"/> of <see cref="VCard"/></returns>
        Task<IEnumerable<VCard>> GetVCardsByLeaseAsync(long leaseId);

        /// <summary>
        /// Updates provided user properties at the card provider.
        /// </summary>
        /// <param name="userToken">The unique token/ID of the user on the card provider network.</param>
        /// <param name="request">Properties to update for the user.</param>
        /// <returns><see cref="CardUserUpdateResponse"/> values present for the user at the card provider after the update.</returns>
        Task<CardUserUpdateResponse> UpdateCardUserAsync(string userToken, CardUserUpdateRequest request);

        /// <summary>
        /// Searches the provider network for user properties.
        /// </summary>
        /// <param name="userToken">Unique token for user on network, usually LeaseId.</param>
        /// <param name="cardProvider">Provider network the user exists on.</param>
        /// <param name="productType">The provider product type.</param>
        /// <returns><see cref="CardUserResponse"/> values present at provider for the user.</returns>
        Task<CardUserResponse> GetCardUserAsync(string userToken, VirtualCardProviderNetwork cardProvider, ProductType productType);

        /// <summary>
        /// Retrieves the wallet batch config from the db.
        /// </summary>
        /// <returns><see cref="WalletBatchConfigResponse"/></returns>
        Task<WalletBatchConfigResponse> GetWalletBatchConfigAsync();

        /// <summary>
        /// Updates the WalletBatchConfig table with the most recent process date.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A <see cref="Task"/>.</returns>
        Task UpdateWalletBatchConfigAsync(UpdateWalletBatchConfigRequest request);

        /// <summary>
        /// Returns a list of vcards cancelled at or before the provided date.
        /// </summary>
        /// <param name="lookupDateTime">The date used to find vcards.</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="CancelledVCard"/></returns>
        Task<IList<CancelledVCard>> GetCancelledVCardsByDateTimeAsync(DateTimeOffset lookupDateTime);

        /// <summary>
        /// Returns the custom, product type record for the storeId provided, if exists.
        /// Should the store not have a custom product type record, then returns the product type record for the virtual card provider.
        /// </summary>
        /// <param name="providerId">The providerId to get product type for, should the store not have a custom record.</param>
        /// <param name="storeId">The storeId to search for a custom product type record for.</param>
        /// <returns><see cref="StoreProductType"/></returns>
        Task<StoreProductType> GetProductTypeAsync(VirtualCardProviderNetwork providerId, int storeId);

        /// <inheritdoc cref="IVirtualPaymentRepository.GetVCardProvidersAsync()"/>
        Task<IEnumerable<VCardProvider>> GetVCardProvidersAsync(bool activeOnly);

        /// <summary>
        /// Creates a new virtual card for the provider and data passed in and saves it to the data repository.
        /// </summary>
        /// <returns></returns>
        Task<VirtualCardResponse> CreateCardAsync(VirtualCardRequest request);

        /// <summary>
        /// Cancels a vcard using the provided data.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A bool reflecting if the cancellation was successful.</returns>
        Task<bool> CancelVCardAsync(CancelVCardRequest request);

        /// <summary>
        /// Determines if the <see cref="VirtualCardRequest.User.PhoneNumber"/> is required
        /// and not present in the request.
        /// </summary>
        /// <param name="request"><see cref="VirtualCardRequest"/></param>
        /// <returns><see cref="bool"/> value noting required phone number is missing from request.</returns>
        Task<bool> IsRequiredUserPhoneMissingAsync(VirtualCardRequest request);
    }
}
