using AutoMapper;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.DTO.Marqeta;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Requests.Marqeta;
using VirtualPaymentService.Model.Responses;
using VirtualPaymentService.Model.Responses.Marqeta;

namespace VirtualPaymentService.Business.Provider
{
    public class MarqetaProvider : ICardProvider, IDigitalWalletProvider
    {
        #region Field
        private readonly IMarqetaCommercialService _commercialService;
        private readonly IMarqetaConsumerService _consumerService;
        private readonly ILogger<MarqetaProvider> _logger;
        private readonly IMapper _mapper;
        private readonly AppSettings _appsettings;
        #endregion Field

        #region Property
        public VPaymentProvider Provider => VPaymentProvider.Marqeta;
        #endregion

        #region Constructor
        public MarqetaProvider
        (
            IMarqetaCommercialService commercialService,
            IMarqetaConsumerService consumerService,
            ILogger<MarqetaProvider> logger,
            IMapper mapper,
            AppSettings appSettings
        )
        {
            _commercialService = commercialService;
            _consumerService = consumerService;
            _logger = logger;
            _mapper = mapper;
            _appsettings = appSettings;
        }
        #endregion Constructor

        #region Method
        /// <inheritdoc />
        public async Task<bool> CancelCardAsync(CancelVCardRequest vCard, ProductType productType)
        {
            return await TransitionVCardAsync(vCard.ReferenceId, MarqetaCardState.TERMINATED, productType);
        }

        /// <inheritdoc />
        public async Task<bool> IsHealthyAsync()
        {
            var success = false;
            try
            {
                var commercialPingResponse = await _commercialService.PingAsync();
                success = commercialPingResponse.Success;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "An error occurred while pinging the Marqeta Commercial api. See the logged exception for more details.", exception: ex);
            }

            try
            {
                var consumerPingResponse = await _consumerService.PingAsync();
                success = success && consumerPingResponse.Success;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "An error occurred while pinging the Marqeta Consumer api. See the logged exception for more details.", exception: ex);
                success = false;
            }

            return success;
        }

        /// <inheritdoc />
        public async Task<ApplePayTokenizationResponse> GetApplePayTokenizationDataAsync(ApplePayProvisioningData data, ProductType productType)
        {
            try
            {
                var marqetaRequestBody = _mapper.Map<MarqetaProvisionApplePayRequest>(data);
                var service = GetMarqetaService(productType);
                var provisionResponse = await service.PostProvisionApplePayAsync(marqetaRequestBody);

                return _mapper.Map<ApplePayTokenizationResponse>(provisionResponse);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                    "An error occurred while attempting to get ApplePay tokenization data from Marqeta. See the logged exception for more details.", exception: ex);

                return new ApplePayTokenizationResponse();
            }
        }

        /// <inheritdoc />
        public async Task<GooglePayTokenizationResponse> GetGooglePayTokenizationDataAsync(GooglePayProvisioningData data, ProductType productType)
        {
            try
            {
                var marqetaRequestBody = _mapper.Map<MarqetaProvisionGooglePayRequest>(data);
                var service = GetMarqetaService(productType);
                var provisionResponse = await service.PostProvisionGooglePayAsync(marqetaRequestBody);

                return _mapper.Map<GooglePayTokenizationResponse>(provisionResponse);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                    "An error occurred while attempting to get Google Pay tokenization data from Marqeta. See the logged exception for more details.", exception: ex);

                return new GooglePayTokenizationResponse();
            }
        }

        /// <inheritdoc />
        public async Task<DigitalWalletTokenResponse> GetDigitalWalletTokensByVCardAsync(string cardToken, ProductType productType)
        {
            try
            {
                var service = GetMarqetaService(productType);
                var searchResponse = await service.GetDigitalWalletTokensByCardToken(cardToken);

                return _mapper.Map<DigitalWalletTokenResponse>(searchResponse);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                    $"An error occurred while attempting to call Marqeta digital wallet token search API using {nameof(cardToken)} value {cardToken} passed in. See the logged exception for more details.", exception: ex);

                throw;
            }
        }

        /// <inheritdoc />
        public async Task<DigitalWalletTokenTransitionResponse> TransitionDigitalWalletTokenAsync(DigitalWalletTokenTransitionRequest request, ProductType productType)
        {
            try
            {
                var marqetaRequestBody = _mapper.Map<MarqetaWalletTokenTransitionRequest>(request);
                var service = GetMarqetaService(productType);
                var transitionResponse = await service.PostDigitalWalletTokenTransitionAsync(marqetaRequestBody);

                // If receive response, accompanied by error code, log a warning.
                if (!string.IsNullOrWhiteSpace(transitionResponse?.ErrorCode))
                {
                    _logger.Log(LogLevel.Warn,
                        $"Attempt to transition {Provider} wallet token encountered " +
                        $"{nameof(transitionResponse.ErrorCode)}: '{transitionResponse.ErrorCode}' - " +
                        $"{nameof(transitionResponse.ErrorMessage)}: '{transitionResponse.ErrorMessage}', " +
                        "but was on the configurable allow list to response successfully.",
                        metadata: new Dictionary<string, object>
                        {
                            { nameof(request.DigitalWalletToken.WalletToken), request?.DigitalWalletToken?.WalletToken },
                            { nameof(request.DigitalWalletToken.CardToken), request?.DigitalWalletToken?.CardToken },
                            { nameof(request.DigitalWalletToken.DeviceType), request?.DigitalWalletToken?.DeviceType },
                            { nameof(request.WalletTokenTransitionStatus), request?.WalletTokenTransitionStatus },
                            { nameof(request.WalletTransitionReasonCode), request?.WalletTransitionReasonCode }
                        });
                }

                return _mapper.Map<DigitalWalletTokenTransitionResponse>(transitionResponse);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                    $"An error occurred while attempting to call Marqeta to transition token for {nameof(request.DigitalWalletToken.WalletToken)} value {request?.DigitalWalletToken?.WalletToken} passed in. See the logged exception for more details.",
                    exception: ex,
                    new Dictionary<string, object>
                    {
                        { nameof(request.DigitalWalletToken.WalletToken), request?.DigitalWalletToken?.WalletToken },
                        { nameof(request.DigitalWalletToken.CardToken), request?.DigitalWalletToken?.CardToken },
                        { nameof(request.DigitalWalletToken.DeviceType), request?.DigitalWalletToken?.DeviceType },
                        { nameof(request.WalletTokenTransitionStatus), request?.WalletTokenTransitionStatus },
                        { nameof(request.WalletTransitionReasonCode), request?.WalletTransitionReasonCode }
                    });

                throw;
            }
        }

        /// <inheritdoc />
        public async Task<CardUserUpdateResponse> UpdateCardUserAsync(string userToken, CardUserUpdateRequest request, ProductType productType)
        {
            try
            {
                var marqetaRequestBody = _mapper.Map<MarqetaUserPutRequest>(request);
                var service = GetMarqetaService(productType);
                var updateResponse = await service.PutUserAsync(userToken, marqetaRequestBody);

                return _mapper.Map<CardUserUpdateResponse>(updateResponse);
            }
            catch (HttpRequestException ex)
            {
                string messageNotFound = $"The {nameof(userToken)} value {userToken} passed in was not located on the virtual card provider network.";
                string messageAllOtherErrorResponseCodes = $"An error occurred while updating user for {nameof(userToken)} value {userToken} passed in.";

                // If this is a 400 from the card provider then the token passed in is not a user that can be located
                // on the providers network. The endpoint at Marqeta does not require any properties to be included in 
                // the request body so keying off a 400 to respond to the caller of this commercialService with a 409 is valid.
                // NOTE: 409 is being returned because Prog standards require a PATCH to respond in this manner when the resource does not exist.
                var statusCodeToReturn = ex.StatusCode == HttpStatusCode.BadRequest ? HttpStatusCode.Conflict : ex.StatusCode;

                // Passing this text along to the caller to better describe issue, especially with not locating a user.
                var problemMessage = ex.StatusCode == HttpStatusCode.BadRequest ? messageNotFound : messageAllOtherErrorResponseCodes;

                _logger.Log(LogLevel.Error,
                    $"An error occurred while attempting to call Marqeta user PUT API. {problemMessage} See the logged exception for more details.", exception: ex);

                // Message and cardUser code will be returned to caller in controller. 
                throw new HttpRequestException(problemMessage, ex, statusCodeToReturn);
            }
        }

        /// <inheritdoc />
        public async Task<CardUserResponse> GetCardUserAsync(string userToken, ProductType productType)
        {
            try
            {
                var cardUser = await GetMarqetaService(productType).GetUserAsync(userToken);

                return _mapper.Map<CardUserResponse>(cardUser);
            }
            catch (HttpRequestException ex)
            {
                // If the user was not located (404) at Marqeta we want to return a message to caller that is a little more detailed.
                string problemMessage = ex.StatusCode == HttpStatusCode.NotFound ? $"The {nameof(userToken)} value {userToken} passed in was not located on the Marqeta {productType} card network." : ex.Message;

                _logger.Log(LogLevel.Error,
                    $"An error occurred while attempting to call Marqeta user GET API. {problemMessage} See the logged exception and metadata for more details.", exception: ex,
                        metadata: new Dictionary<string, object>
                        {
                            { nameof(userToken), userToken },
                            { nameof(productType), productType },
                        });

                throw new HttpRequestException(problemMessage, ex, ex.StatusCode);
            }
        }

        /// <inheritdoc />
        public async Task<VirtualCardResponse> CreateCardAsync(VirtualCardRequest request, ProductType productType)
        {
            try
            {
                var providerSetting = _appsettings.GetCardProviderSetting(request.VCardProviderId, productType);

                // Create or update the user.
                var user = await CreateUpdateUser(_mapper.Map<MarqetaUserPostRequest>(request), productType, providerSetting.MarqetaAccountHolderGroupToken);

                // Conditional return,if the request didnt contain specific user info
                // then return back null. If present then return back the user data we have at Marqeta.
                // NOTE: If user data is not included in the request the user created just includes
                //       metadata for the user with a unique identifier using LeaseId.
                CardUserUpdateResponse userResponse = request.User != null ? _mapper.Map<CardUserUpdateResponse>(user) : null;

                var service = GetMarqetaService(productType);

                // Create a new card.
                var card = await service.PostCardAsync(new MarqetaCardRequest()
                {
                    Token = GenerateCardToken((int)request.StoreId),
                    UserToken = user.Token,
                    CardProductToken = providerSetting.VirtualCardProductToken
                });

                if (providerSetting.SetInitalPin)
                {
                    var pinToken = await service.PostPinControlTokenAsync(new MarqetaPinControlTokenRequest { CardToken = card.Token, ControlTokenType = "SET_PIN" });
                    await service.PutPinAsync(new MarqetaPinRequest { ControlToken = pinToken.ControlToken, Pin = request.User.PhoneNumber[^4..] });
                }

                return new VirtualCardResponse()
                {
                    LeaseId = (int)request.LeaseId,
                    User = userResponse,
                    Card = _mapper.Map<VCard>(new MarqetaCardVirtualCard()
                    {
                        CardCreated = card,
                        CardRequest = request,
                    })
                };
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"An unexpected error occurred while trying to create the virtual card on the Marqeta network for lease {request.LeaseId}, see the logged exception for more details.", exception: ex);
                throw;
            }
        }

        #endregion Method

        #region Private Methods
        /// <summary>
        /// Creates or updates a user on the Marqeta network.
        /// </summary>
        /// <remarks>
        /// If the user exists it will update the user, if not will create a new user.
        /// </remarks>
        /// <param name="request"><see cref="MarqetaUserPostRequest"/></param>
        /// <param name="productType">The <see cref="ProductType"/> the user should be created/updated on.</param>
        /// <returns><see cref="MarqetaUserResponse"/></returns>
        private async Task<MarqetaUserResponse> CreateUpdateUser(MarqetaUserPostRequest request, ProductType productType, string marqetaAccountHolderGroupToken)
        {
            var service = GetMarqetaService(productType);

            try
            {
                if (!string.IsNullOrWhiteSpace(marqetaAccountHolderGroupToken))
                {
                    request.AccountHolderGroupToken = marqetaAccountHolderGroupToken;
                }

                // Call the post endpoint and if successful return the cardUser.
                return await service.PostUserAsync(request);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    _logger.Log(LogLevel.Info, $"The Marqeta user with the token/LeaseId {request.Token} already exists on the network, performing an update of the user instead.");

                    // If we get HTTP 409 (Conflict) from the post the user already exists on the 
                    // network. Call the put endpoint to update the existing user.
                    return await service.PutUserAsync(request.Token, _mapper.Map<MarqetaUserPutRequest>(request));
                }

                _logger.Log(LogLevel.Error, $"The call to Marqeta POST user endpoint was not successful, see the logged exception for more details.", exception: ex);

                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"An unexpected error occurred while trying to create the user on the Marqeta network, see the logged exception for more details.", exception: ex);

                throw;
            }
        }

        /// <summary>
        /// Generates a unique token to identify the card at Marqeta.
        /// </summary>
        /// <param name="storeId">The store related to the lease.</param>
        /// <returns>Unique token as <see cref="string"/></returns>
        private string GenerateCardToken(int storeId)
        {
            var tokenMaxLength = _appsettings.MarqetaCardTokenMaxLength;

            if (tokenMaxLength < 1)
            {
                throw new ArgumentException($"The setting {nameof(_appsettings.MarqetaCardTokenMaxLength)} in appsetting.json must be greater than zero!");
            }

            string token = $"{storeId}-{Guid.NewGuid()}";

            if (token.Length > tokenMaxLength)
            {
                // Truncate string to max length.
                token = token[..tokenMaxLength];
            }

            return token;
        }

        /// <summary>
        /// Transitions a vcard to the provided card status
        /// </summary>
        /// <param name="cardToken">The cardToken or referenceId of the VCard to transition.</param>
        /// <param name="newCardStatus">The vCard status to update the vcard with.</param>
        /// <param name="productType">Product type for which the virtual card was created with.</param>
        /// <returns>A <see cref="bool"/> reflecting if the transition was successful.</returns>
        private async Task<bool> TransitionVCardAsync(string cardToken, MarqetaCardState newCardStatus, ProductType productType)
        {
            try
            {
                var request = new MarqetaTransitionCardRequest
                {
                    CardToken = cardToken,
                    CardStatus = newCardStatus
                };

                var service = GetMarqetaService(productType);

                _ = await service.PostTransitionVCardAsync(request);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                    $"Failed to transition vcard to {nameof(MarqetaTransitionCardRequest.CardStatus)} of {newCardStatus} using {nameof(MarqetaProvider)}",
                    exception: ex);

                return false;
            }
        }

        /// <summary>
        /// Returns the service to call based on the <see cref="ProductType"/> passed in. 
        /// </summary>
        /// <param name="productType">The card product type.</param>
        /// <exception cref="NotSupportedException">Exception when <see cref="ProductType"/> passed in is not supported.</exception>
        private IMarqetaService GetMarqetaService(ProductType productType)
        {
            switch (productType)
            {
                case ProductType.Commercial:
                    return _commercialService;
                case ProductType.Consumer:
                    return _consumerService;
                default:
                    throw new NotSupportedException($"The {nameof(ProductType)} of {productType} is not currently supported! There may be an issue in the appsettings.json for the combination of provider {Provider} and product {productType}");
            }
        }
        #endregion Private Methods
    }
}
