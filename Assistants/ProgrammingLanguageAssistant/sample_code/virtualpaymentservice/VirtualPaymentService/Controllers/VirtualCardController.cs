using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgLeasing.System.Logging.Contract;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Model.Domain;
using VirtualPaymentService.Model.DTO;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests;
using VirtualPaymentService.Model.Responses;

namespace VirtualPaymentService.Controllers
{
    [Route("vcard/")]
    [ApiController]
    public class VirtualCardController : ControllerBase
    {
        #region Field
        private readonly TelemetryClient _telemetryClient;
        private readonly ILogger<VirtualCardController> _logger;
        private readonly IVirtualCardProvider _virtualCardProvider;
        #endregion Field

        #region Property
        public string RequestMethodWithRoute => $"{Request?.Method} - {Request?.Path.Value}";
        #endregion Property

        #region Constructor
        public VirtualCardController
        (
            TelemetryClient telemetryClient,
            ILogger<VirtualCardController> logger,
            IVirtualCardProvider virtualCardProvider
        )
        {
            _telemetryClient = telemetryClient;
            _logger = logger;
            _virtualCardProvider = virtualCardProvider;
        }
        #endregion Constructor

        #region Action
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetVCardsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Gets a filtered list of vcards, using the provided query params.")]
        public async Task<IActionResult> GetVCardsAsync([FromQuery] GetVCardsRequest request)
        {
            try
            {
                _logger.Log(LogLevel.Info, RequestMethodWithRoute, metadata: new Dictionary<string, object>
                {
                    {nameof(GetVCardsRequest), JsonSerializer.Serialize(request)},
                });

                var result = await _virtualCardProvider.GetVCardsByFilterAsync(request);

                _logger.Log(LogLevel.Info, $"Successfully retrieved {result.VCards.Count} {result.VCards}");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                            $"An exception occurred while attempting to {nameof(GetVCardsResponse)}. See logged exception for more details.",
                            ex);

                return Problem(title: $"An exception occurred while attempting to {nameof(GetVCardsResponse)}.");
            }

        }

        /// <summary>
        /// Creates a new virtual card for a lease based on the request passed in.
        /// </summary>
        /// <returns>
        /// Success returns <see cref="VirtualCardResponse"/>
        /// </returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(VirtualCardResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Creates a new virtual card for a lease based on the request passed in.")]
        public async Task<IActionResult> PostCreateCardAsync([FromBody] VirtualCardRequest request)
        {
            try
            {
                if (await _virtualCardProvider.IsRequiredUserPhoneMissingAsync(request))
                {
                    ModelState.AddModelError($"{nameof(request.User)}.{nameof(request.User.PhoneNumber)}", $"The {nameof(request.User.PhoneNumber)} field is required when creating a virtual card for this lease.");
                    return BadRequest(ModelState);
                }

                var response = await _virtualCardProvider.CreateCardAsync(request);

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (HttpRequestException ex)
            {
                string problemMessage = $"An unexpected error occurred while creating new virtual card for lease {request.LeaseId}.";

                // Return detail to caller but not logging error since this was already logged in the provider.
                return Problem(title: problemMessage, statusCode: (int)ex.StatusCode);
            }
            catch (Exception ex)
            {
                // Logging error for exceptions that are not related to calling the card provider service endpoint.
                _logger.Log(LogLevel.Error, $"An exception occurred while creating new virtual card for lease {request.LeaseId}. Please see the exception in this log for more details.", exception: ex);

                return Problem($"An exception occurred while creating new virtual card for lease {request.LeaseId}.");
            }
        }

        /// <summary>
        /// Gets wallet tokenization data for ApplePay.
        /// </summary>
        /// <returns><see cref="ApplePayTokenizationResponse"/> with a 201 or 500 status code.</returns>
        [HttpPost]
        [Route("token/applewallet")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApplePayTokenizationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Get wallet tokenization data for ApplePay.", Description = "Provisions a card for ApplePay through a given provider. (e.g. Marqeta)")]
        public async Task<IActionResult> PostTokenizeApplePayVCardAsync([FromBody] ApplePayTokenizationRequest request)
        {
            try
            {
                _logger.Log(LogLevel.Info, $"ApplePay tokenization request received for lease: {request.LeaseId}");

                var response = await _virtualCardProvider.GetApplePayTokenizationDataAsync(request);

                if (string.IsNullOrWhiteSpace(response.ActivationData))
                {
                    _telemetryClient.TrackEvent("Unsuccessful ApplePay Tokenization");

                    return Problem("Unsuccessful ApplePay Tokenization", statusCode: StatusCodes.Status500InternalServerError);
                }

                _logger.Log(LogLevel.Info, $"ApplePay tokenization request for lease {request.LeaseId} was successful");

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message, ex);
                _telemetryClient.TrackException(ex);

                return Problem(title: $"ApplePay tokenization for LeaseId {request.LeaseId} threw an exception.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Gets wallet tokenization data for Google Pay.
        /// </summary>
        /// <returns><see cref="GooglePayTokenizationResponse"/> with a 201 or 500 status code.</returns>
        [HttpPost("token/googlewallet")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GooglePayTokenizationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Get wallet tokenization data for Google Pay.", Description = "Provisions a card for Google Pay through a given provider. (e.g. Marqeta)")]
        public async Task<IActionResult> PostTokenizeGooglePayVCardAsync([FromBody] GooglePayTokenizationRequest request)
        {
            try
            {
                _logger.Log(LogLevel.Info, $"Google Pay tokenization request received for lease: {request.LeaseId}");

                var response = await _virtualCardProvider.GetGooglePayTokenizationDataAsync(request);

                if (response?.PushTokenizeRequestData?.UserAddress == null)
                {
                    _telemetryClient.TrackEvent("Unsuccessful Google Pay Tokenization");

                    return Problem("Unsuccessful Google Pay Tokenization", statusCode: StatusCodes.Status500InternalServerError);
                }

                _logger.Log(LogLevel.Info, $"Google Pay tokenization request for lease {request.LeaseId} was successful");

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message, ex);
                _telemetryClient.TrackException(ex);

                return Problem(title: $"Google Pay tokenization for LeaseId {request.LeaseId} threw an exception.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Gets all VCards for a given lease.
        /// </summary>
        /// <returns><see cref="IEnumerable{VCard}"/> with a 200 or 500 status code.</returns>
        [HttpGet]
        [Route("{leaseId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<VCard>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Get all VCards for a given lease")]
        public async Task<IActionResult> GetVCardsByLeaseAsync([FromRoute] long leaseId)
        {
            try
            {
                var vCards = await _virtualCardProvider.GetVCardsByLeaseAsync(leaseId);

                return Ok(vCards);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"An exception occurred while retrieving vcards for {nameof(leaseId)} {leaseId}. See the exception in this log for more details.", exception: ex);
                _telemetryClient.TrackException(ex);

                return Problem($"An error occurred while attempting to retrieve vcards for {nameof(leaseId)} {leaseId}");
            }

        }

        /// <summary>
        /// Gets all digital wallet tokens related to the virtual card token passed in.
        /// </summary>
        /// <param name="cardToken">
        /// Card token to search against.
        /// </param>
        /// <returns>
        /// <see cref="DigitalWalletTokenResponse"/>
        /// </returns>
        [HttpGet]
        [Route("{cardToken}/wallet/token")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Get all digital wallet tokens for given virtual card.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully located 0-n number of digital wallet tokens for given virtual card.", typeof(DigitalWalletTokenResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Returned when the card token passed in is not found on the card providers network.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error encountered when processing GET request.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetDigitalWalletTokensByVCardAsync([FromRoute] string cardToken)
        {
            try
            {
                var digitalWalletTokens = await _virtualCardProvider.GetDigitalWalletTokensByVCardAsync(cardToken);

                return Ok(digitalWalletTokens);
            }
            catch (HttpRequestException ex)
            {
                string messageCardNotFound = $"The {nameof(cardToken)} value {cardToken} passed in was not located on the virtual card provider network.";
                string messageAllOtherErrorResponseCodes = $"An error occurred while locating digital wallet tokens for {nameof(cardToken)} value {cardToken} passed in.";

                // If this is a 404 from the card provider then the token passed in is not a card that can be located
                // on the providers network. Passing this text along to the caller to better describe issue with value passed in.
                var problemMessage = ex.StatusCode == HttpStatusCode.NotFound ? messageCardNotFound : messageAllOtherErrorResponseCodes;

                // Return detail to caller but not logging error since this was already logged in the card provider.
                return Problem(title: problemMessage, statusCode: (int)ex.StatusCode);
            }
            catch (Exception ex)
            {
                // Logging error for exceptions that are not related to calling the card provider service endpoint.
                _logger.Log(LogLevel.Error, $"An exception occurred while locating digital wallet tokens for {nameof(cardToken)} value {cardToken} passed in. Please see the exception in this log for more details.", exception: ex);

                return Problem($"An error occurred while locating digital wallet tokens for {nameof(cardToken)} value {cardToken} passed in.");
            }
        }

        /// <summary>
        /// Transitions Digital Wallet Token to specified WalletTokenTransitionStatus of type - DigitalWalletTokenStatus
        /// </summary>
        /// <returns><see cref="DigitalWalletTokenTransitionResponse"/> with a 201, 500 status code.</returns>
        [HttpPost]
        [Route("wallet/tokentransition")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DigitalWalletTokenTransitionResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Transition Wallet Tokens to specified status")]
        public async Task<IActionResult> PostWalletTokenTransitionAsync([FromBody] DigitalWalletTokenTransitionRequest request)
        {
            try
            {
                var response = await _virtualCardProvider.TransitionDigitalWalletTokenAsync(request);

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (HttpRequestException ex)
            {
                var dataNotFoundMsg = "Virtual card provider could not find the " +
                    $"{nameof(request.DigitalWalletToken.WalletToken)}: {request.DigitalWalletToken.WalletToken} or " +
                    $"{nameof(request.WalletTransitionReasonCode)}: {request.WalletTransitionReasonCode}, for the associated " +
                    $"{nameof(request.DigitalWalletToken.CardToken)}: {request.DigitalWalletToken.CardToken}.";

                var httpExceptionMsg =
                    $"An exception occurred while attempting to transition the following " +
                    $"{nameof(request.DigitalWalletToken.WalletToken)}: {request.DigitalWalletToken.WalletToken}, for the associated " +
                    $"{nameof(request.DigitalWalletToken.CardToken)}: {request.DigitalWalletToken.CardToken}.";

                var problemMessage = ex.StatusCode == HttpStatusCode.NotFound ? dataNotFoundMsg : httpExceptionMsg;

                // Respond with status code, which was logged and thrown, in virtual card provider
                return Problem(title: problemMessage, statusCode: Convert.ToInt32(ex.StatusCode));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"PostWalletTokenTransitionAsync caught an exception - {ex.Message}.", ex,
                    new Dictionary<string, object>
                    {
                        { nameof(request.DigitalWalletToken.WalletToken), request?.DigitalWalletToken?.WalletToken },
                        { nameof(request.DigitalWalletToken.CardToken), request?.DigitalWalletToken?.CardToken }
                    });
                _telemetryClient.TrackException(ex);

                return Problem(
                    title:
                    $"PostWalletTokenTransitionAsync caught an exception - {ex.Message}, while processing request for " +
                    $"{nameof(request.DigitalWalletToken.WalletToken)}: {request?.DigitalWalletToken?.WalletToken}, " +
                    $"{nameof(request.DigitalWalletToken.CardToken)}: {request?.DigitalWalletToken?.CardToken}.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Updates user properties at the card provider. 
        /// </summary>
        /// <param name="userToken">Unique identifier at card provider network.</param>
        /// <param name="request">Properties to update for the user.</param>
        /// <returns>Success 200 with body of <see cref="CardUserUpdateResponse"/></returns>
        [HttpPatch]
        [Route("user/{userToken}")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Updates user properties at the card provider.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully updated user, returns current values from card provider.", typeof(CardUserUpdateResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned when request body does not pass validation.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Returned when user token is not located on the card provider network - tries all card products.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error encountered when processing request.", typeof(ProblemDetails))]
        public async Task<IActionResult> PatchCardUserAsync([FromRoute] string userToken, [FromBody] CardUserUpdateRequest request)
        {
            try
            {
                var response = await _virtualCardProvider.UpdateCardUserAsync(userToken, request);

                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                // Return detail to caller but not logging error since this was already logged in the card provider.
                return Problem(title: ex.Message, statusCode: (int)ex.StatusCode);
            }
            catch (Exception ex)
            {
                // Logging error for exceptions that are not related to calling the card provider service endpoint.
                _logger.Log(LogLevel.Error, $"An exception occurred while updating the user for {nameof(userToken)} value {userToken} passed in. Please see the exception in this log for more details.", exception: ex);

                return Problem($"An error occurred while updating the user for {nameof(userToken)} value {userToken} passed in.");
            }
        }

        /// <summary>
        /// Searches the card provider for the user properties set on the network.
        /// </summary>
        /// <param name="userToken">The unique token for the user on the network, usually lease ID.</param>
        /// <param name="cardProvider">The card provider network to search.</param>
        /// <param name="productType">The card provider product type.</param>
        [HttpGet]
        [Route("user/{userToken}")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Retrieves user properties at the card provider.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully located and returned user values from card provider.", typeof(CardUserResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Required query string parameter/s not submitted in request.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Did not locate the user on the card provider network.", typeof(ProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error encountered when processing request.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetCardUserAsync(
            [FromRoute] string userToken,
            [FromQuery][Required] VirtualCardProviderNetwork cardProvider,
            [FromQuery][Required] ProductType productType)
        {
            try
            {
                // The Enum supports 0/Null but this endpoint will not, returns 400 with details to caller.
                if (productType == ProductType.Null)
                {
                    ModelState.AddModelError($"{nameof(ProductType)}.{nameof(ProductType.Null)}", $"The {nameof(ProductType)} must be provided, the value of zero/null is not supported in this endpoint.");
                    return BadRequest(ModelState);
                }

                var response = await _virtualCardProvider.GetCardUserAsync(userToken, cardProvider, productType);

                return Ok(response);
            }
            catch (HttpRequestException ex)
            {
                // Return detail to caller but not logging error since this was already logged in the card provider.
                return Problem(title: ex.Message, statusCode: (int)ex.StatusCode);
            }
            catch (Exception ex)
            {
                // Logging error for exceptions that are not related to calling the card provider service endpoint.
                _logger.Log(LogLevel.Error, $"An exception occurred while searching for user data on card provider network.", exception: ex,
                    metadata: new Dictionary<string, object>
                    {
                        { nameof(userToken), userToken },
                        { nameof(cardProvider), cardProvider },
                        { nameof(productType), productType },
                    });

                return Problem($"An error occurred while searching for user data on the {cardProvider} provider {productType} network for {nameof(userToken)} value {userToken} passed in.");
            }
        }

        /// <summary>
        /// Returns the custom, product type record for the storeId provided, if exists.
        /// Should the store not have a custom product type record, then returns the product type record for the virtual card provider.
        /// </summary>
        /// <param name="providerId">The providerId to fetch product type for, should the store not have a custom record.</param>
        /// <param name="storeId">The storeId to search for a custom product type record for.</param>
        /// <returns>Success 200 with body of <see cref="IEnumerable{T}"/> of <see cref="VCardProvider"/></returns>
        [HttpGet]
        [Route("productType")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Gets custom product type record for store or default of provider if store record does not exist.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully updated user, returns current values from card provider.", typeof(IEnumerable<StoreProductType>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "No content found for given parameters.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned when an invalid value is provided for parameters.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error encountered when processing request.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetVCardProductTypeAsync([FromQuery][Required] VirtualCardProviderNetwork providerId, [FromQuery] int storeId)
        {
            try
            {
                var response = await _virtualCardProvider.GetProductTypeAsync(providerId, storeId);

                return response != null ? Ok(response) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.Log(
                    LogLevel.Error,
                    $"An exception occurred while attempting to get store product type for {nameof(storeId)}: {storeId}.",
                    exception: ex);

                return Problem($"An error occurred while getting store product type for {nameof(storeId)}: {storeId}. See logs for more details.");
            }
        }

        /// <summary>
        /// Gets all vcard providers, or only active ones depending on the query paramater.
        /// </summary>
        /// <param name="activeOnly">Bool to determine whether all or only active providers are returned.</param>
        /// <returns>Success 200 with body of <see cref="IEnumerable{T}"/> of <see cref="VCardProvider"/></returns>
        [HttpGet]
        [Route("providers")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Gets vcard provider list.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns collection of vcard provider settings.", typeof(IEnumerable<VCardProvider>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned when an invalid value is provided for activeOnly.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error encountered when processing request.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetVCardProvidersAsync([FromQuery] bool activeOnly = false)
        {
            try
            {
                var response = await _virtualCardProvider.GetVCardProvidersAsync(activeOnly);

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Logging error for exceptions that are not related to calling the card provider service endpoint.
                _logger.Log(
                    LogLevel.Error,
                    $"An exception occurred while attempting to get vcard providers. Please see the exception in this log for more details.",
                    exception: ex);

                return Problem($"An error occurred while getting vcard providers. See logs for more details.");
            }
        }

        /// <summary>
        /// Gets the wallet batch configuration.
        /// </summary>
        /// <returns>
        /// <see cref="DigitalWalletTokenResponse"/>
        /// </returns>
        [HttpGet]
        [Route("wallet/config")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Get wallet batch configuration(s).")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully responding with the batch configuration", typeof(WalletBatchConfigResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error while retrieving the configuration.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetWalletBatchConfigAsync()
        {
            try
            {
                _logger.Log(LogLevel.Info, $"{RequestMethodWithRoute}: In progress");

                var configResponse = await _virtualCardProvider.GetWalletBatchConfigAsync();

                var metadata = new Dictionary<string, object>()
                {
                    { "ResponseBody", JsonSerializer.Serialize(configResponse) }
                };
                _logger.Log(LogLevel.Info, $"{RequestMethodWithRoute}: Success", metadata: metadata);

                return Ok(configResponse);
            }
            catch (Exception ex)
            {
                // Logging error for exceptions that are not related to calling the card provider service endpoint.
                _logger.Log(LogLevel.Error, $"{RequestMethodWithRoute}: An exception occurred while retrieving a {nameof(WalletBatchConfigResponse)}. Please see the exception in this log for more details.", exception: ex);

                return Problem($"An exception occurred while retrieving the {nameof(WalletBatchConfigResponse)}.");
            }
        }

        /// <summary>
        /// Updates the wallet batch configuration.
        /// </summary>
        /// <returns>
        /// A 201 status code if successful, 500 if not.
        /// </returns>
        [HttpPut]
        [Route("wallet/config")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Update the wallet batch configuration.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Successfully updated the batch configuration.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Malformed request.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error while updating the configuration.", typeof(ProblemDetails))]
        public async Task<IActionResult> PutWalletBatchConfigAsync([FromBody] UpdateWalletBatchConfigRequest request)
        {
            try
            {
                var metadata = new Dictionary<string, object>
                {
                    {$"{nameof(request.LastTerminationProcessedDate)}", request.LastTerminationProcessedDate },
                    {$"{nameof(request.UpdatedBy)}", request.UpdatedBy }
                };
                _logger.Log(LogLevel.Info, $"{RequestMethodWithRoute}: In progress", metadata: metadata);

                await _virtualCardProvider.UpdateWalletBatchConfigAsync(request);

                _logger.Log(LogLevel.Info, $"{RequestMethodWithRoute}: Success");

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                    $"{RequestMethodWithRoute}: An exception occurred while processing {nameof(UpdateWalletBatchConfigRequest)}. Please see the exception in this log for more details.",
                    exception: ex);

                return Problem($"An exception occurred while processing the {nameof(UpdateWalletBatchConfigRequest)}.");
            }
        }

        /// <summary>
        /// Gets all vcards cancelled after or at the given DateTimeOffset.
        /// </summary>
        /// <param name="utcDateTime">The date to compare against.</param>
        /// <returns>
        /// A 200 status code if successful, 500 if not.
        /// </returns>
        [HttpGet]
        [Route("cancelled/{utcDateTime}")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Gets all vcards cancelled after or at the given DateTimeOffset.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully retrieved the cancelled vcards.", typeof(IList<CancelledVCard>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Malformed request.")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error while updating the configuration.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetCancelledVCardsByDateTimeAsync([FromRoute] DateTimeOffset utcDateTime)
        {
            try
            {
                _logger.Log(LogLevel.Info,
                    $"{RequestMethodWithRoute}: In progress, with datetime {utcDateTime.LocalDateTime}");

                var vCards = await _virtualCardProvider.GetCancelledVCardsByDateTimeAsync(utcDateTime);

                _logger.Log(LogLevel.Info, $"{RequestMethodWithRoute}: Success. Found {vCards.Count} cancelled vcard(s)");

                return Ok(vCards);
            }
            catch (Exception ex)
            {
                var errorMessage = $"An exception occurred while retrieving vcards updated on or after {utcDateTime.LocalDateTime}.";
                _logger.Log(LogLevel.Error,
                    $"{RequestMethodWithRoute}: {errorMessage} Please see the exception in this log for more details.",
                    exception: ex);

                return Problem(errorMessage);
            }
        }

        /// <summary>
        /// Cancels a VCard using the provided information.
        /// </summary>
        /// <param name="request">The vcard cancellation request.</param>
        /// <returns>Success 201</returns>
        [HttpPost]
        [Route("cancel")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Cancel the provided VCard.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Successfully cancelled the vcard.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned when request body does not pass validation.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error encountered when processing request.", typeof(ProblemDetails))]
        public async Task<IActionResult> PostCancelVCardAsync([FromBody] CancelVCardRequest request)
        {
            try
            {
                _logger.Log(LogLevel.Debug, $"Canceling VCard", metadata: new Dictionary<string, object>
                {
                    {nameof(CancelVCardRequest), request},
                });

                var cancelIsSuccess = await _virtualCardProvider.CancelVCardAsync(request);

                return cancelIsSuccess ?
                    Created("", null) :
                    Problem(title: "VCard Provider Failed",
                        detail: $"VCardProvider {nameof(request.VCardProviderId)} responded with responded with an error code.");
            }
            catch (Exception ex)
            {
                // Logging error for exceptions that are not related to calling the card provider service endpoint.
                _logger.Log(LogLevel.Error,
                            $"An exception occurred while processing {nameof(CancelVCardRequest)} for {nameof(CancelVCardRequest.ReferenceId)} {request.ReferenceId}",
                            exception: ex);

                return Problem(title: "An unknown exception occurred while attempting to cancel the VCard",
                               detail: "See logged errors for more details.");
            }
        }

        [HttpGet]
        [Route("transaction/authorization")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Retrieves saved authorizations for the given parameter/s.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully located saved authorizations for virtual card.", typeof(GetVirtualCardAuthorizationsResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned when request url does not include required parameter/s.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error encountered when processing request.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetVirtualCardAuthorizationsAsync([FromQuery][Required] int vCardId, [FromServices] IVPayTransactionProvider trxProvider)
        {
            try
            {
                _logger.Log(LogLevel.Info, RequestMethodWithRoute, metadata: new Dictionary<string, object>
                {
                    {nameof(vCardId), vCardId},
                });

                return Ok(await trxProvider.GetVirtualCardAuthorizationsAsync(vCardId));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                            $"An exception occurred while retrieving saved authorization records for {nameof(vCardId)} {vCardId}!",
                            exception: ex);

                return Problem(title: $"An unknown exception occurred while attempting to retrieve saved authorization records for {nameof(vCardId)} {vCardId}.",
                               detail: "See logged errors for more details.");
            }
        }

        /// <summary>
        /// Adds a new authorization record, associated with the specified vcard.
        /// </summary>
        /// <param name="request">The authorization request. <see cref="VCardPurchaseAuthRequest"/></param>
        /// <param name="trxProvider">An instance of the <see cref="IVPayTransactionProvider"/></param>
        /// <returns><see cref="StatusCodes.Status204NoContent"/> if a duplicate transaction is sent or no VCard is found. <see cref="StatusCodes.Status201Created"/> otherwise.</returns>
        [HttpPost]
        [Route("transaction/authorization")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Adds a transaction for the given VCard.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Successfully inserted the transaction.", typeof(VCardPurchaseAuthResponse))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "The transaction has already been inserted.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned when request body does not pass validation.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error encountered when processing request.", typeof(ProblemDetails))]
        public async Task<IActionResult> PostVCardAuthorizationAsync(
            [FromBody] VCardPurchaseAuthRequest request,
            [FromServices] IVPayTransactionProvider trxProvider)
        {
            try
            {
                _logger.Log(LogLevel.Info, RequestMethodWithRoute, metadata: new Dictionary<string, object>
                {
                    {nameof(VCardPurchaseAuthRequest), JsonSerializer.Serialize(request)},
                });

                var result = await trxProvider.AddVCardAuthorizationAsync(request);

                return result.SoftFail ? NoContent() : Created(string.Empty, result.Response);
            }
            catch (Exception ex)
            {
                // Logging error for exceptions that are not related to calling the card provider service endpoint.
                _logger.Log(LogLevel.Error,
                            $"An exception occurred while processing {nameof(VCardPurchaseAuthRequest)} for {nameof(VCardPurchaseAuthRequest.ReferenceId)} {request.ReferenceId}",
                            exception: ex);

                return Problem(title: "An unknown exception occurred while attempting to add authorization for VCard",
                               detail: "See logged errors for more details.");
            }
        }

        [HttpGet]
        [Route("transaction/settlement")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Retrieves saved settlements for the given parameter/s.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Successfully located saved settlements for lease.", typeof(GetSettlementTransactionsResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned when request url does not include required parameter/s.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error encountered when processing request.", typeof(ProblemDetails))]
        public async Task<IActionResult> GetSettlementTransactionsAsync([FromQuery][Required] int leaseId, [FromServices] IVPayTransactionProvider trxProvider)
        {
            try
            {
                _logger.Log(LogLevel.Info, RequestMethodWithRoute, metadata: new Dictionary<string, object>
                {
                    {nameof(leaseId), leaseId},
                });

                return Ok(await trxProvider.GetSettlementTransactionsAsync(leaseId));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                            $"An exception occurred while retrieving saved settlement records for {nameof(leaseId)} {leaseId}!",
                            exception: ex);

                return Problem(title: $"An unknown exception occurred while attempting to retrieve saved settlement records for {nameof(leaseId)} {leaseId}.",
                               detail: "See logged errors for more details.");
            }
        }

        /// <summary>
        /// Saves settlement transactions for virtual cards.
        /// </summary>
        /// <param name="request">Settlement transaction request <see cref="SettlementTransactionRequest"/></param>
        /// <param name="trxProvider">An instance of the <see cref="IVPayTransactionProvider"/></param>
        /// <returns>Returns <see cref="StatusCodes.Status201Created"/> when settlement transaction/s created successfully.</returns>
        [HttpPost]
        [Route("transaction/settlement")]
        [Produces("application/json")]
        [SwaggerOperation(Summary = "Adds settlement transactions for virtual cards.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Successfully inserted the settlement transactions.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned when request body does not pass validation.", typeof(ValidationProblemDetails))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Unexpected error encountered when processing request.", typeof(ProblemDetails))]
        public async Task<IActionResult> PostSettlementTransactionAsync(
            [FromBody] SettlementTransactionRequest request,
            [FromServices] IVPayTransactionProvider trxProvider)
        {
            try
            {
                _logger.Log(LogLevel.Info, RequestMethodWithRoute, metadata: new Dictionary<string, object>
                {
                    {nameof(SettlementTransactionRequest), JsonSerializer.Serialize(request)},
                });

                await trxProvider.AddSettlementTransactionAsync(request);

                return StatusCode(StatusCodes.Status201Created);
            }
            catch (AggregateException ex)
            {
                _logger.Log(LogLevel.Error, $"An error occurred saving some or all settlement transactions. Details of failed transactions were previously logged.", ex);

                // Just need the first part of the AggregateException message returned to caller.
                var baseMessageEnd = ex.Message.IndexOf("~", StringComparison.Ordinal);

                return Problem(title: "An error occurred saving some or all settlement transactions.",
                               detail: ex.Message.Substring(0, baseMessageEnd > 0 ? baseMessageEnd : ex.Message.Length));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error,
                            $"An exception occurred while processing the {nameof(SettlementTransactionRequest)}, request body logged in details.",
                            exception: ex,
                            metadata: new Dictionary<string, object>
                            {
                                {nameof(SettlementTransactionRequest), JsonSerializer.Serialize(request)},
                            });

                return Problem(title: "An unknown exception occurred while attempting to save settlement transactions.",
                               detail: "See logged errors for more details.");
            }
        }
        #endregion Action
    }
}
