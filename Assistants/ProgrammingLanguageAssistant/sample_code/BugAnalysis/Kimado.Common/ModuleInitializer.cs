// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the ModuleInitializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kimado.Common.Contracts;
using Kimado.Common.Services;
using Kimado.Core;
using Kimado.Core.Contracts;

namespace Kimado.Common
{
    /// <summary>
    /// The module initializer.
    /// </summary>
    public static class ModuleInitializer
    {
        /// <summary>
        /// Gets or sets a value indicating whether registered.
        /// </summary>
        private static bool Registered { get; set; }

        /// <summary>
        /// The register.
        /// </summary>
        public static void Register()
        {
            if (!Registered)
            {
                IServiceContainer serviceContainer = ServiceContainer.Instance;
                serviceContainer.PostRegistration += ServiceContainerPostRegistration;
                Registered = true;
            }
        }

        /// <summary>
        /// The service container_ post registration.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void ServiceContainerPostRegistration(object sender, RegistrationEventArgs e)
        {
            IServiceContainer serviceContainer = e.ServiceContainer;
            serviceContainer.FunctionalServices.Register<ICertificateService>(() => { return new CertificateService(serviceContainer); });
            serviceContainer.FunctionalServices.Register<ISecureWebClientService>(() => { return new SecureWebClientService(serviceContainer); });
            serviceContainer.FunctionalServices.Register<ICryptoRandomNumberFunctionalService>(() => { return new CryptoRandomNumberFunctionalService(serviceContainer); });
        }
    }
}