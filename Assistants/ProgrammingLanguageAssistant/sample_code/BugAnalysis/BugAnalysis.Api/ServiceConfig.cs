using Kimado.Core.Contracts;

namespace BugAnalysis.Api
{
    public class ServiceConfig
    {
        /// <summary>
        /// Initialize will init the IOC service container
        /// </summary>
        /// <param name="container">Service container.</param>
        public static void Initialize(IServiceContainer container)
        {
            // Initialize the WebApi IOC container.  The FunctionalService ServiceConfig is responsible for configuring the Functional Service layer.
            if (!container.Initialized)
            {
				/* Initialize the Functional service layer */
                /* container.RegisterServices = c => { FunctionalService.ServiceConfig.Instance.Initialize(c); }; */
                container.Initialize();
            }
        }
    }
    
}
