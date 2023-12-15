using Kimado.Core.Contracts;
using BugAnalysis.DataService.Interfaces;
using BugAnalysis.DataService.Services;

namespace BugAnalysis.DataService
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
                    return serviceConfig ??= new ServiceConfig();
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
            // .............................
            // Register data services
            // .............................
            container.DataServices.Register<IBugAnalysisDataService>(() => new BugAnalysisDataService(container));
        }

        #endregion
    }
}
