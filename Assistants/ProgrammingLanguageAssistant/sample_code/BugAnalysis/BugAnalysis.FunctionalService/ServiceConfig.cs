using Kimado.Core.Contracts;
using Kimado.Common.Services;
using Kimado.Common.Contracts;
using BugAnalysis.FunctionalService.Interfaces;
using BugAnalysis.FunctionalService.Services;

namespace BugAnalysis.FunctionalService
{
    public class ServiceConfig : IServiceConfiguration
    {
        /// <summary>
        /// The service config.
        /// </summary>
        private static IServiceConfiguration serviceConfig;

        /// <summary>
        /// Gets or sets <see cref="IServiceConfiguration"/> instance.
        /// </summary>
        public static IServiceConfiguration Instance
        {
            get
            {
                lock (typeof(ServiceConfig))
                {
                    if (serviceConfig == null)
                    {
                        serviceConfig = new ServiceConfig();
                    }

                    return serviceConfig;
                }
            }

            set
            {
                lock (typeof(ServiceConfig))
                {
                    serviceConfig = value;
                }
            }
        }

        #region Implementation of IServiceConfiguration

        /// <summary>
        /// Initialize the IOC container
        /// </summary>
        /// <param name="container">IServiceContainer container</param>
        public void Initialize(IServiceContainer container)
        {
            // Register the data services
            DataService.ServiceConfig.Instance.Initialize(container);

            // .............................
            // Register functional services
            // .............................
			container.FunctionalServices.Register<IBugAnalysisFunctionalService>(() => new BugAnalysisFunctionalService(container));
			container.FunctionalServices.Register<ICertificateService>(() => new CertificateService(container));
        }

        #endregion
    }
}
