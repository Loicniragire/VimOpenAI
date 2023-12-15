// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceContainer.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the IServiceContainer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kimado.Core.Contracts
{
    /// <summary>
    /// The ServiceContainer interface.
    /// </summary>
    public interface IServiceContainer
    {
        /// <summary>
        /// Event fired after registration so that non-mainline code can access the factory registries
        /// </summary>
        event EventHandler<RegistrationEventArgs> PostRegistration;

        /// <summary>
        /// Gets the service container
        /// </summary>
        IServiceContainer Container { get; }

        /// <summary>
        /// Gets or sets the services via a delegate allowing the client to define which services
        /// are registered for this instance.
        /// </summary>
        Action<IServiceContainer> RegisterServices { get; set; }
        Action<IServiceContainer> RegisterInstance { get; set; }

        /// <summary>
        /// Gets the data services
        /// </summary>
        IDataServiceFactory DataServices { get; }

        /// <summary>
        /// Gets the functional services
        /// </summary>
        IFunctionalServiceFactory FunctionalServices { get; }

        /// <summary>
        /// Gets the Cache services
        /// </summary>
        ICacheServiceFactory CacheServices { get; }

        /// <summary>
        /// Gets or sets the caller context Guid default value for the service
        /// </summary>
        Guid ContextId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether initialized.
        /// </summary>
        bool Initialized { get; set; }

        /// <summary>
        /// Initialize will initialize the service container and call the registration actions
        /// </summary>
        /// <returns>Returns true if initialization was successful.</returns>
        bool Initialize();

        /// <summary>
        /// The auto register.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        void AutoRegister(string prefix);

        /// <summary>
        /// The register as proxy.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <typeparam name="T">Proxy type
        /// </typeparam>
        void RegisterAsProxy<T>(string prefix);

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <typeparam name="T">Service container
        /// </typeparam>
        void Register<T>(string prefix);
    }
}