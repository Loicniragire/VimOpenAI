﻿using System.Collections.Generic;
using System.Configuration;
using VirtualPaymentService.Model.Enums;

namespace VirtualPaymentService.Business.Configuration
{
    public class AppSettings
    {
        public string ENV { get; set; }
        public IDictionary<string, string> SecretServerKeyNames { get; set; }
        public IDictionary<string, string> Urls { get; set; }
        public IDictionary<string, string> Endpoints { get; set; }
        public List<CardProviderSetting> CardProviderSettings { get; set; }

        /// <summary>
        /// The number of retries to execute when calling the secret server for a key value.
        /// </summary>
        /// <remarks>
        /// If the setting is not in the config this will default to zero, no retries)
        /// </remarks>
        public int SecretServerGetSecretRetryLimit { get; set; }

        /// <summary>
        /// Calls to the mobile wallet provider return mocked responses for the 
        /// service endpoints that honor this setting.
        /// </summary>
        /// <remarks>
        /// Allows to mimic successful responses in lower lanes (QA-Demo) that
        /// do not function in the external wallet providers test environment.
        /// </remarks>
        public bool UseMockedMobileWalletResponse { get; set; }

        /// <summary>
        /// The max number of wallet tokens a provider should return to our GET request.
        /// </summary>
        /// <remarks>
        /// If the setting is not in the config this will default to 5.
        /// </remarks>
        public int DigitalWalletTokenCountLimit { get; set; } = 5;

        /// <summary>
        /// The maximum length the unique token that identifies a virtual card can be
        /// </summary>
        /// <remarks>
        /// The default value (36) can be overridden in the config if present.
        /// </remarks>
        public int MarqetaCardTokenMaxLength { get; set; } = 36;

        /// <summary>
        /// List of Marqeta error codes to allow successful wallet token transition response.
        /// </summary>
        public List<string> MarqetaWalletTransitionOverrideErrorCodes { get; set; } = new ();

        public AppSettings()
        {
            Urls = new Dictionary<string, string>();
            Endpoints = new Dictionary<string, string>();
            SecretServerKeyNames = new Dictionary<string, string>();
            CardProviderSettings = new List<CardProviderSetting>();
        }

        /// <summary>
        /// Retrieves a url by its key from the appsettings.
        /// </summary>
        /// <param name="urlKey">The key to the url in appsettings.json</param>
        /// <returns>A string url if the key is found, an empty string if it is not.</returns>
        public string GetUrl(string urlKey)
        {
            return Urls.TryGetValue(urlKey, out var url) ? url : string.Empty;
        }

        /// <summary>
        /// Retrieves a endpoint by its key from the appsettings.
        /// </summary>
        /// <param name="endpointKey">The key to the endpoint in appsettings.json</param>
        /// <returns>The endpoint if itis found, an empty string if not.</returns>
        public string GetEndpoint(string endpointKey)
        {
            return Endpoints.TryGetValue(endpointKey, out var endpoint) ? endpoint : string.Empty;
        }

        /// <summary>
        /// Retrieves a secret server key name from appsettings.json
        /// </summary>
        /// <param name="secretServerKeyName">Key name from the SecretServerKeyNames section.</param>
        /// <returns>Key name from appsettings.json or empty string if not found.</returns>
        public string GetSecretServerKeyName(string secretServerKeyName)
        {
            return SecretServerKeyNames.TryGetValue(secretServerKeyName, out var keyName) ? keyName : string.Empty;
        }

        /// <summary>
        /// Retrieves <see cref="CardProviderSetting"/> from CardProviderSettings section of appsettings for a provider/product combination.
        /// </summary>
        /// <param name="provider">The card provider.</param>
        /// <param name="productType">The card product type.</param>
        /// <returns><see cref="CardProviderSetting"/> located for the provider/product combination.</returns>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public CardProviderSetting GetCardProviderSetting(VirtualCardProviderNetwork provider, ProductType productType)
        {
            var cardProviderSetting = CardProviderSettings.Find(s => s.CardProvider == provider && s.ProductType == productType);

            if (cardProviderSetting == null)
            {
                var missingProviderConfigMessage = $"Did not locate the {nameof(CardProviderSetting)} in CardProviderSettings of appsetting.json for provider {provider} and product type {productType}!";
                throw new ConfigurationErrorsException(missingProviderConfigMessage);
            }

            return cardProviderSetting;
        }
    }
}