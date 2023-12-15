using System.Threading.Tasks;
using VirtualPaymentService.Model.DTO.Marqeta;
using VirtualPaymentService.Model.Requests.Marqeta;
using VirtualPaymentService.Model.Responses;
using VirtualPaymentService.Model.Responses.Marqeta;

namespace VirtualPaymentService.Business.Interface
{
    public interface IMarqetaService
    {
        /// <summary>
        /// Hits Marqeta ping endpoint.
        /// </summary>
        /// <returns><see cref="Task"/> of <see cref="MarqetaPingResponse"/></returns>
        Task<MarqetaPingResponse> PingAsync();

        /// <summary>
        /// Uses the Marqeta API to provision a card for ApplePay
        /// and gets tokenization data for the digital Wallet.
        /// </summary>
        /// <param name="request">Data provided by Apple to use in the request.</param>
        /// <returns><see cref="Task"/> of <see cref="MarqetaProvisionApplePayResponse"/></returns>
        Task<MarqetaProvisionApplePayResponse> PostProvisionApplePayAsync(MarqetaProvisionApplePayRequest request);

        /// <summary>
        /// Uses the Marqeta API to provision a card for Google Pay
        /// and gets tokenization data for the digital wallet.
        /// </summary>
        /// <param name="request">Data provided by Google to use in the request.</param>
        /// <returns><see cref="Task"/> of <see cref="MarqetaProvisionGooglePayResponse"/></returns>
        Task<MarqetaProvisionGooglePayResponse> PostProvisionGooglePayAsync(MarqetaProvisionGooglePayRequest request);

        /// <summary>
        /// Uses the Marqeta API to search for digital wallet tokens for card passed in.
        /// </summary>
        /// <remarks>Calls the Marqeta API https://www.marqeta.com/docs/core-api/digital-wallets-management#_list_digital_wallet_tokens_for_card </remarks>
        /// <param name="cardToken">The card token to search against.</param>
        /// <returns><see cref="Task"/> of <see cref="MarqetaDigitalWalletTokensForCardResponse"/></returns>
        Task<MarqetaDigitalWalletTokensForCardResponse> GetDigitalWalletTokensByCardToken(string cardToken);

        /// <summary>
        /// Uses Marqeta API to transition a digital wallet token state to Active(GREEN), Suspended(YELLOW), Terminated(RED)
        /// </summary>
        /// <param name="request">Data included for specifying wallet token and transition state</param>
        /// <returns><see cref="Task"/> of <see cref="MarqetaWalletTokenTransitionResponse"/></returns>
        Task<MarqetaWalletTokenTransitionResponse> PostDigitalWalletTokenTransitionAsync(MarqetaWalletTokenTransitionRequest request);

        /// <summary>
        /// Uses Marqeta API to update provided properties of the user that would be associated to a virtual card.
        /// </summary>
        /// <remarks>Marqeta API https://www.marqeta.com/docs/core-api/users#_update_user </remarks>
        /// <param name="userToken">Unique Marqeta user token, for virtual cards token value is LeaseId.</param>
        /// <param name="request">User properties to update at Marqeta.</param>
        /// <returns><see cref="Task"/> of <see cref="MarqetaUserResponse"/></returns>
        Task<MarqetaUserResponse> PutUserAsync(string userToken, MarqetaUserPutRequest request);

        /// <summary>
        /// Uses Marqeta API to create a user that will be associated to the virtual card.
        /// </summary>
        /// <remarks>Marqeta API https://www.marqeta.com/docs/core-api/users#_create_user </remarks>
        /// <param name="request">Properties to create user at Marqeta.</param>
        /// <returns><see cref="Task"/> of <see cref="MarqetaUserResponse"/></returns>
        Task<MarqetaUserResponse> PostUserAsync(MarqetaUserPostRequest request);

        /// <summary>
        /// Uses Marqeta API to get the current user properties.
        /// </summary>
        /// <remarks>
        /// Marqeta API https://www.marqeta.com/docs/core-api/users#getUsersToken
        /// </remarks>
        /// <param name="userToken">Unique Marqeta user token, for virtual cards token value is LeaseId.</param>
        /// <returns><see cref="Task"/> of <see cref="MarqetaUserResponse"/></returns>
        Task<MarqetaUserResponse> GetUserAsync(string userToken);

        /// <summary>
        /// Uses Marqeta API to create virtual card.
        /// </summary>
        /// <remarks>Marqeta API https://www.marqeta.com/docs/core-api/cards#postCards </remarks>
        /// <param name="request">Properties to create card at Marqeta.</param>
        /// <returns><see cref="Task"/> of <see cref="MarqetaCardResponse"/></returns>
        Task<MarqetaCardResponse> PostCardAsync(MarqetaCardRequest request);

        /// <summary>
        /// Uses Marqeta API to transition a vcard state to Active, Suspended, Terminated.
        /// Will not throw if the VCard is already in the provided status.
        /// </summary>
        /// <param name="request">Data included for specifying vcard and transition state</param>
        /// <returns><see cref="Task"/> of <see cref="MarqetaTransitionCardResponse"/></returns>
        Task<MarqetaTransitionCardResponse> PostTransitionVCardAsync(MarqetaTransitionCardRequest request);

        /// <summary>
        /// Creates a control token on network to allow subsequent pin operation using generated token.
        /// </summary>
        /// <remarks>
        /// Marqeta API https://www.marqeta.com/docs/core-api/pins#postPinsControltoken
        /// </remarks>
        /// <param name="request">The card and pin token type to generate.</param>
        /// <returns><see cref="Task"/> of <see cref="MarqetaPinControlTokenResponse"/></returns>
        Task<MarqetaPinControlTokenResponse> PostPinControlTokenAsync(MarqetaPinControlTokenRequest request);

        /// <summary>
        /// Sets or updates the PIN for a card.
        /// </summary>
        /// <remarks>
        /// Marqeta API https://www.marqeta.com/docs/core-api/pins#putPins
        /// </remarks>
        /// <param name="request">The control token and PIN number to set.</param>
        Task PutPinAsync(MarqetaPinRequest request);
    }
}