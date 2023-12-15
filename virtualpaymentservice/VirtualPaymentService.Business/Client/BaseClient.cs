using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Interface;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Business.Client
{
    /// <summary>
    /// BaseClient to be inherited from by HttpClients.
    /// Provides an <see cref="InitializeClient(HttpClient, AppSettings, ISecretConfigurationService)"/> method
    /// to add basic authorization headers, if an instance of <see cref="ISecretConfigurationService"/> is provided, 
    /// and will set the <see cref="HttpClient.BaseAddress"/>.
    /// </summary>
    public abstract class BaseClient
    {
        #region Field
        /// <summary>
        /// The provider configuration.
        /// </summary>
        private readonly CardProviderSetting _cardProviderSetting;
        #endregion Field

        #region Property
        /// <summary>
        /// The card provider we are connecting to.
        /// </summary>
        protected abstract VirtualCardProviderNetwork CardProvider { get; }

        /// <summary>
        /// The card product type.
        /// </summary>
        protected abstract ProductType CardProductType { get; }
        #endregion Property

        #region Constructor
        /// <summary>
        /// Set HttpClient default request headers, excluding a basic authorization header.
        /// </summary>
        /// <param name="client"><see cref="HttpClient"/></param>
        /// <param name="appSettings"><see cref="AppSettings"/></param>
        protected BaseClient(HttpClient client, AppSettings appSettings)
        {
            _cardProviderSetting = appSettings.GetCardProviderSetting(CardProvider, CardProductType);

            InitializeClient(client);
        }

        /// <summary>
        /// Set HttpClient default request headers, including a basic authorization header.
        /// </summary>
        /// <param name="client"><see cref="HttpClient"/></param>
        /// <param name="appSettings"><see cref="AppSettings"/></param>
        /// <param name="secretConfigurationService"><see cref="ISecretConfigurationService"/></param>
        protected BaseClient(HttpClient client, AppSettings appSettings, ISecretConfigurationService secretConfigurationService)
        {
            _cardProviderSetting = appSettings.GetCardProviderSetting(CardProvider, CardProductType);

            InitializeClient(client, secretConfigurationService);
        }
        #endregion Constructor

        #region Method
        /// <summary>
        /// Sets the <see cref="HttpClient"/>'s BaseAddress using the inheriting class <see cref="ServiceUrl"/> property.
        /// Sets the default accept and content type of a request to application/json.
        /// Optionally adds basic authorization to the default request headers.
        /// </summary>
        /// <param name="client">
        /// An instance of <see cref="HttpClient"/>.
        /// </param>
        /// <param name="secretConfigurationService">
        /// An instance of <see cref="ISecretConfigurationService"/>. If omitted, no basic authorization header will be added to the default headers.
        /// </param>
        private void InitializeClient(HttpClient client, ISecretConfigurationService secretConfigurationService = null)
        {
            client.BaseAddress = new Uri(_cardProviderSetting.BaseUrl);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Optionally add basic authorization header
            if (secretConfigurationService != null)
            {
                client.DefaultRequestHeaders.Authorization = CreateAuthHeader(secretConfigurationService);
            }
        }
        #endregion Method

        #region Helper
        private AuthenticationHeaderValue CreateAuthHeader(ISecretConfigurationService secretConfigurationService)
        {
            var basicAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{secretConfigurationService.GetSecretValueForKey(_cardProviderSetting.ApiUserKeyName)}:{secretConfigurationService.GetSecretValueForKey(_cardProviderSetting.ApiPasswordKeyName)}"));
            return new AuthenticationHeaderValue("Basic", basicAuth);
        }
        #endregion Helper
    }
}