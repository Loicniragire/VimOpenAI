using Polly;
using ProgLeasing.Platform.SecretConfiguration;
using ProgLeasing.System.Logging.Contract;
using System;
using System.Collections.Generic;
using System.Configuration;
using VirtualPaymentService.Business.Configuration;
using VirtualPaymentService.Business.Interface;

namespace VirtualPaymentService.Business.Service
{
    /// <summary>
    /// Handles retrieval of secret values from Progressive secret store.
    /// </summary>
    public class SecretConfigurationService : ISecretConfigurationService
    {
        private readonly ILogger<SecretConfigurationService> _logger;
        private readonly AppSettings _appSettings;
        private readonly ISecretManager _secretManager;
        private readonly Dictionary<string, string> _secretValues;

        public SecretConfigurationService(ISecretManager secretManager, AppSettings appSettings, ILogger<SecretConfigurationService> logger)
        {
            _logger = logger;
            _appSettings = appSettings;
            _secretManager = secretManager;
            _secretValues = new Dictionary<string, string>();
            GetAllSecretValues();
        }

        /// <inheritdoc/>
        public string GetSecretValueForKey(string secretKeyName)
        {
            return _secretValues.TryGetValue(secretKeyName, out var secretValue) ? secretValue : string.Empty;
        }

        /// <summary>
        /// Gets all of the secret values for the card providers defined in <see cref="AppSettings.CardProviderSettings"/>
        /// </summary>
        private void GetAllSecretValues()
        {
            try
            {
                var cardProviderSettings = _appSettings.CardProviderSettings;

                foreach (var cardProviderSetting in cardProviderSettings)
                {
                    if (string.IsNullOrWhiteSpace(cardProviderSetting.ApiUserKeyName) || string.IsNullOrWhiteSpace(cardProviderSetting.ApiPasswordKeyName))
                    {
                        var missingConfigKeyMessage = $"Did not locate the required key name/s {nameof(cardProviderSetting.ApiUserKeyName)} {nameof(cardProviderSetting.ApiPasswordKeyName)} or a value for one of the keys in the CardProviderSettings section of appsetting.json for provider {cardProviderSetting.CardProvider} and product type {cardProviderSetting.ProductType}!";
                        throw new ConfigurationErrorsException(missingConfigKeyMessage);
                    }

                    _secretValues.Add(cardProviderSetting.ApiUserKeyName, GetSecretValue(cardProviderSetting.ApiUserKeyName));
                    _secretValues.Add(cardProviderSetting.ApiPasswordKeyName, GetSecretValue(cardProviderSetting.ApiPasswordKeyName));
                }
            }
            catch (Exception ex)
            {
                // Added text to the error so a Splunk alert can be created. At this point there is a bigger problem
                // than just retrying the call to the secrets store.
                _logger.Log(LogLevel.Error, $"Failure to retrieve value from secrets store after all configured retries! The service is in an unhealthy state and root cause must be addressed.  Last error message encountered: {ex.Message}", ex);
                // Re-throw the exception since these values are required for the service to work properly.
                throw;
            }
        }

        /// <summary>
        /// Locates the secret value from the secret store.
        /// </summary>
        /// <param name="secretKeyName">The key name for the secret at the secret store.</param>
        /// <returns>Value from the secrets store.</returns>
        private string GetSecretValue(string secretKeyName)
        {
            string secretValue = string.Empty;

            // Using Polly retry the number of times configured.  If the setting 
            // does not exist in appsetting.json retry is set to zero (no retries).
            Policy
            .Handle<ArgumentException>()
            .Retry(_appSettings.SecretServerGetSecretRetryLimit, onRetry: (exception, retryCount) =>
            {
                // Log initial/previous try as warning to not fire Splunk alert for secret failure.
                _logger.Log(LogLevel.Warn, $"Retrying call to {nameof(ISecretManager)} to retrieve secret value for key {secretKeyName}, retry count {retryCount}. Previous execution error: {exception.Message}"
                    , exception);
            })
            .Execute(() => secretValue = GetSecretValueFromSecretServer(secretKeyName));

            _logger.Log(LogLevel.Info, $"Successfully retrieved secret value for key {secretKeyName}.");

            return secretValue;
        }

        /// <summary>
        /// Retrieves the secret value from the secret server store.
        /// </summary>
        /// <param name="secretKeyName">The key name for the secret value to retrieve.</param>
        /// <returns>The secret value or throws <see cref="ArgumentException"/> if there was an issue retrieving the value.</returns>
        private string GetSecretValueFromSecretServer(string secretKeyName)
        {
            var secretValue = _secretManager.GetSecret(secretKeyName);

            if (!secretValue.Success)
            {
                var secretValueNotFoundMessage = string.IsNullOrWhiteSpace(secretValue.ErrorMessage) ? $"Unknown error when trying to retrieve secret value for key {secretKeyName}." : secretValue.ErrorMessage;
                throw new ArgumentException(secretValueNotFoundMessage);
            }

            return secretValue.Value;
        }
    }
}
