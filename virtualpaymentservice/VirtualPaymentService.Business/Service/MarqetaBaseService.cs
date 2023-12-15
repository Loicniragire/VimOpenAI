using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Client;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Business.Mocks;
using VirtualPaymentService.Business.Service.Helpers;
using VirtualPaymentService.Model.DTO.Marqeta;
using VirtualPaymentService.Model.Enums;
using VirtualPaymentService.Model.Requests.Marqeta;
using VirtualPaymentService.Model.Responses;
using VirtualPaymentService.Model.Responses.Marqeta;

namespace VirtualPaymentService.Business.Service
{
    /// <summary>
    /// Class to encapsulate the endpoint calls to Marqeta. They have a different
    /// base URL for consumer and commercial cards but the request/response contracts 
    /// and behaviors are the same between them.
    /// </summary>
    public abstract class MarqetaBaseService : BaseClient, IMarqetaService
    {
        #region Field
        private readonly HttpClient _client;
        private readonly AppSettings _appsettings;
        private readonly JsonSerializerOptions _jsonOptions;
        #endregion Field

        #region Property
        protected override VirtualCardProviderNetwork CardProvider => VirtualCardProviderNetwork.Marqeta;

        private string PingEndpoint => _appsettings.GetEndpoint("MarqetaPing");
        private string ProvisionApplePayEndpoint => _appsettings.GetEndpoint("MarqetaProvisionApplePay");
        private string ProvisionGooglePayEndpoint => _appsettings.GetEndpoint("MarqetaProvisionGooglePay");
        private string GetDigitalWalletTokensByCardTokenEndpoint => _appsettings.GetEndpoint("MarqetaGetDigitalWalletTokensByCardToken");
        private string TransitionDigitalWalletTokenEndpoint => _appsettings.GetEndpoint("MarqetaTransitionDigitalWalletToken");
        private string UsersEndpoint => _appsettings.GetEndpoint("MarqetaUsers");
        private string CreateCardEndpoint => _appsettings.GetEndpoint("MarqetaCreateCard");
        private string TransitionCardEndpoint => _appsettings.GetEndpoint("MarqetaTransitionCard");
        private string ControlTokenEndpoint => _appsettings.GetEndpoint("MarqetaControlToken");
        private string SetPinEndpoint => _appsettings.GetEndpoint("MarqetaSetPin");
        #endregion Property

        #region Constructor
        protected MarqetaBaseService(HttpClient client, AppSettings appSettings, ISecretConfigurationService secretConfigurationService)
            : base(client, appSettings, secretConfigurationService)
        {
            _client = client;
            _appsettings = appSettings;

            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }
        #endregion Constructor

        #region Method
        /// <inheritdoc/>
        public async Task<MarqetaPingResponse> PingAsync()
        {
            return await _client.GetFromJsonAsync<MarqetaPingResponse>(PingEndpoint, options: _jsonOptions);
        }

        /// <inheritdoc/>
        public async Task<MarqetaProvisionApplePayResponse> PostProvisionApplePayAsync(MarqetaProvisionApplePayRequest request)
        {
            if (_appsettings.UseMockedMobileWalletResponse)
            {
                return MarqetaMobileWalletMocks.PostProvisionApplePaySuccessMockResponse(request);
            }
            else
            {
                HttpResponseMessage response = await _client.PostAsJsonAsync(ProvisionApplePayEndpoint, request);
                ValidateSuccessStatusCode(response);
                return await response.Content.ReadFromJsonAsync<MarqetaProvisionApplePayResponse>();
            }
        }

        /// <inheritdoc/>
        public async Task<MarqetaProvisionGooglePayResponse> PostProvisionGooglePayAsync(MarqetaProvisionGooglePayRequest request)
        {
            if (_appsettings.UseMockedMobileWalletResponse)
            {
                return MarqetaMobileWalletMocks.PostProvisionGooglePaySuccessMockResponse(request);
            }
            else
            {
                using HttpResponseMessage response = await _client.PostAsJsonAsync(ProvisionGooglePayEndpoint, request);
                ValidateSuccessStatusCode(response);
                return await response.Content.ReadFromJsonAsync<MarqetaProvisionGooglePayResponse>();
            }
        }

        /// <inheritdoc/>
        public async Task<MarqetaDigitalWalletTokensForCardResponse> GetDigitalWalletTokensByCardToken(string cardToken)
        {
            if (_appsettings.UseMockedMobileWalletResponse)
            {
                return MarqetaMobileWalletMocks.GetDigitalWalletTokensByCardTokenSuccessMockResponse(cardToken);
            }
            else
            {
                var resultLimit = _appsettings.DigitalWalletTokenCountLimit;
                var endpoint = RouteUtil.FormatRoute(GetDigitalWalletTokensByCardTokenEndpoint, cardToken, resultLimit.ToString());
                return await _client.GetFromJsonAsync<MarqetaDigitalWalletTokensForCardResponse>(endpoint, options: _jsonOptions);
            }
        }

        /// <inheritdoc/>
        public async Task<MarqetaWalletTokenTransitionResponse> PostDigitalWalletTokenTransitionAsync(MarqetaWalletTokenTransitionRequest request)
        {
            if (_appsettings.UseMockedMobileWalletResponse)
            {
                return MarqetaMobileWalletMocks.PostDigitalWalletTokenTransitionSuccessMockResponse(request);
            }

            using HttpResponseMessage response = await _client.PostAsJsonAsync(TransitionDigitalWalletTokenEndpoint, request);

            var responseWithErrorProps = await response.Content.ReadFromJsonAsync<MarqetaWalletTokenTransitionResponse>();

            // If non-successful status code but error code deemed non-critical, specify request rejected in response.
            // Otherwise, ValidateSuccessStatusCode will throw on non-successful status codes.
            if (!response.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(responseWithErrorProps?.ErrorCode) &&
                _appsettings.MarqetaWalletTransitionOverrideErrorCodes.Contains(responseWithErrorProps.ErrorCode))
            {
                responseWithErrorProps.FulfillmentStatus = "REJECTED";
                responseWithErrorProps.WalletTransitionType = "state.requested";
            }
            else
            {
                ValidateSuccessStatusCode(response);
            }

            return responseWithErrorProps;
        }

        /// <inheritdoc/>
        public async Task<MarqetaUserResponse> PutUserAsync(string userToken, MarqetaUserPutRequest request)
        {
            string endpoint = RouteUtil.FormatRoute(UsersEndpoint, userToken);
            using HttpResponseMessage response = await _client.PutAsJsonAsync(endpoint, request, options: _jsonOptions);
            ValidateSuccessStatusCode(response);
            return await response.Content.ReadFromJsonAsync<MarqetaUserResponse>();
        }

        /// <inheritdoc/>
        public async Task<MarqetaUserResponse> PostUserAsync(MarqetaUserPostRequest request)
        {
            using HttpResponseMessage response = await _client.PostAsJsonAsync(UsersEndpoint, request, options: _jsonOptions);
            ValidateSuccessStatusCode(response);
            return await response.Content.ReadFromJsonAsync<MarqetaUserResponse>();
        }

        /// <inheritdoc/>
        public async Task<MarqetaUserResponse> GetUserAsync(string userToken)
        {
            var endpoint = RouteUtil.FormatRoute(UsersEndpoint, userToken);
            return await _client.GetFromJsonAsync<MarqetaUserResponse>(endpoint, options: _jsonOptions);
        }

        /// <inheritdoc/>
        public async Task<MarqetaCardResponse> PostCardAsync(MarqetaCardRequest request)
        {
            using HttpResponseMessage response = await _client.PostAsJsonAsync(CreateCardEndpoint, request, options: _jsonOptions);
            ValidateSuccessStatusCode(response);
            return await response.Content.ReadFromJsonAsync<MarqetaCardResponse>();
        }

        public async Task<MarqetaTransitionCardResponse> PostTransitionVCardAsync(MarqetaTransitionCardRequest request)
        {
            using HttpResponseMessage response = await _client.PostAsJsonAsync(TransitionCardEndpoint, request, options: _jsonOptions);
            ValidateSuccessStatusCode(response);
            return await response.Content.ReadFromJsonAsync<MarqetaTransitionCardResponse>();
        }

        public async Task<MarqetaPinControlTokenResponse> PostPinControlTokenAsync(MarqetaPinControlTokenRequest request)
        {
            using HttpResponseMessage response = await _client.PostAsJsonAsync(ControlTokenEndpoint, request, options: _jsonOptions);
            ValidateSuccessStatusCode(response);
            return await response.Content.ReadFromJsonAsync<MarqetaPinControlTokenResponse>();
        }

        public async Task PutPinAsync(MarqetaPinRequest request)
        {
            using HttpResponseMessage response = await _client.PutAsJsonAsync(SetPinEndpoint, request, options: _jsonOptions);
            ValidateSuccessStatusCode(response);
        }
        #endregion Method

        #region Private
        /// <summary>
        /// Validates if response from HTTP action was successful using <see cref="HttpResponseMessage.IsSuccessStatusCode"/>. 
        /// If not throws <see cref="HttpRequestException"/> which includes response body if returned.
        /// </summary>
        /// <param name="response"><see cref="HttpResponseMessage"/> to validate.</param>
        /// <exception cref="HttpRequestException"></exception>
        private static void ValidateSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = response.Content?.ReadAsStringAsync().Result.Replace("\n", "").Replace("\r", "");
                var responseBodyMessage = string.IsNullOrEmpty(responseBody) ? string.Empty : $" with returned response body of {responseBody}";
                var errorMessage = $"Received unsuccessful HTTP status code {(int)response.StatusCode} while attempting {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}{responseBodyMessage}";

                throw new HttpRequestException(errorMessage, null, response.StatusCode);
            }
        }
        #endregion Private

    }
}
