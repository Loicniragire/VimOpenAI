using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VirtualPaymentService.Business.Interface;

namespace VirtualPaymentService.HealthChecks
{
    /// <summary>
    /// Healthcheck to validate a successful communication with our VCard providers.
    /// </summary>
    public class VCardProviderHealthCheck : IHealthCheck
    {
        #region Field
        private readonly IEnumerable<Type> _cardProviders;
        private readonly IServiceProvider _serviceProvider;
        private bool isHealthy = true;
        #endregion Field

        #region Constructor
        /// <summary>
        /// Constructor used by the <see cref="HealthCheckService"/>
        /// </summary>
        /// <param name="serviceProvider"></param>
        public VCardProviderHealthCheck(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            // Return all classes which implement the ICardProvider interface
            _cardProviders = Assembly.GetAssembly(typeof(ICardProvider))
                .GetTypes()
                .Where(t => typeof(ICardProvider).IsAssignableFrom(t))
                .Where(t => !(t.IsInterface || t.IsAbstract));
        }

        public VCardProviderHealthCheck(IServiceProvider serviceProvider, IEnumerable<Type> cardProviders)
        {
            _serviceProvider = serviceProvider;
            _cardProviders = cardProviders;
        }
        #endregion Constructor

        #region Method
        /// <summary>
        /// Method for performing the VCard Provider healthcheck
        /// </summary>
        /// <param name="context"><see cref="HealthCheckContext"/></param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns>A <see cref="Task"/> of a <see cref="HealthCheckResult"/></returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {

            IReadOnlyDictionary<string, object> providerResults = await PingAllProvidersAsync();

            var healthCheckResult = isHealthy ? HealthCheckResult.Healthy(data: providerResults) : HealthCheckResult.Unhealthy(data: providerResults);

            return healthCheckResult;
        }

        /// <summary>
        /// Executes the "PingAsync" method for each provider and returns if the request was successful.
        /// </summary>
        /// <returns>A <see cref="Task"/> of a <see cref="IReadOnlyDictionary{TKey, TValue}"/></returns>
        private async Task<IReadOnlyDictionary<string, object>> PingAllProvidersAsync()
        {
            var pingTasks = new List<Task<bool>>();
            var providerNames = new List<string>();

            // Calls the PingAsync() method for each CardProvider and adds it a List of tasks.
            foreach (Type providerType in _cardProviders)
            {
                var provider = (ICardProvider)ActivatorUtilities.CreateInstance(_serviceProvider, providerType);
                providerNames.Add(provider.Provider.ToString());
                pingTasks.Add(provider.IsHealthyAsync());
            }

            // Wait for all requests to complete
            await Task.WhenAll(pingTasks);

            var providerResults = new Dictionary<string, object>();
            // Use an enumerator to line up provider names with their ping response
            using (IEnumerator<Task<bool>> taskIterator = pingTasks.GetEnumerator())
            {
                foreach (string name in providerNames)
                {
                    if (taskIterator.MoveNext())
                    {
                        bool isSuccess = taskIterator.Current.Result;
                        providerResults.Add($"{name} communication success", isSuccess);
                        isHealthy = isSuccess && isHealthy;
                    }
                    else
                    {
                        providerResults.Add($"{name} communication success", false);
                        isHealthy = false;
                    }
                }
            }

            return providerResults;
        }
        #endregion Method
    }
}