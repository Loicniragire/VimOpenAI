using System.Threading.Tasks;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Business.Interface
{
    /// <summary>
    /// Interface for Digital Wallet providers. Implements <see cref="ICardProvider"/>.
    /// </summary>
    public interface IDigitalWalletProvider
    {
        /// <summary>
        /// Sends a provisioning request to the provider and returns AppleWallet tokenization data.
        /// </summary>
        /// <param name="data">Payload for requesting wallet tokenization data from Apple.</param>
        /// <param name="productType">Product type for which the virtual card was created with.</param>
        /// <returns>Task of <see cref="ApplePayTokenizationResponse"/>. ApplePayTokenizationResponse.Success will be false if the request failed.</returns>
        Task<ApplePayTokenizationResponse> GetApplePayTokenizationDataAsync(ApplePayProvisioningData data, ProductType productType);

        /// <summary>
        /// Sends a provisioning request to the provider and returns Google Wallet tokenization data.
        /// </summary>
        /// <param name="data">Payload for requesting wallet tokenization data from Google.</param>
        /// <param name="productType">Product type for which the virtual card was created with.</param>
        /// <returns>Task of <see cref="GooglePayTokenizationResponse"/>.</returns>
        Task<GooglePayTokenizationResponse> GetGooglePayTokenizationDataAsync(GooglePayProvisioningData data, ProductType productType);

        /// <summary>
        /// Queries provider to locate all digital wallet tokens for given card token.
        /// </summary>
        /// <param name="cardToken">Card token to search against.</param>
        /// <param name="productType">Product type for which the virtual card was created with.</param>
        /// <returns>Task of <see cref="DigitalWalletTokenResponse"/>.</returns>
        Task<DigitalWalletTokenResponse> GetDigitalWalletTokensByVCardAsync(string cardToken, ProductType productType);

        /// <summary>
        /// Transitions digital wallet token to the transition status supplied and returns the digital wallet transition token.
        /// </summary>
        /// <param name="request">Contains the wallet token, new transition status, and reason code required for request to wallet provider</param>
        /// <param name="productType">Product type for which the virtual card was created with.</param>
        /// <returns><see cref="DigitalWalletTokenTransitionResponse"/></returns>
        Task<DigitalWalletTokenTransitionResponse> TransitionDigitalWalletTokenAsync(DigitalWalletTokenTransitionRequest request, ProductType productType);

        /// <summary>
        /// Updates user properties at the card provider.
        /// </summary>
        /// <param name="userToken">The unique token/ID of the user on the card provider network.</param>
        /// <param name="request">Properties to update for the user.</param>
        /// <param name="productType">Product type for which the virtual card was created with.</param>
        /// <returns><see cref="CardUserUpdateResponse"/> values present for the user at the card provider after the update.</returns>
        public Task<CardUserUpdateResponse> UpdateCardUserAsync(string userToken, CardUserUpdateRequest request, ProductType productType);
    }
}