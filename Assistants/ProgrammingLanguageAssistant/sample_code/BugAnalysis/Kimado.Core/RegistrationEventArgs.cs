// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationEventArgs.cs" company="Kimado">
//   Property of Fiduciary Exchange
// </copyright>
// <summary>
//   Defines the RegistrationEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kimado.Core.Contracts;

namespace Kimado.Core
{
    /// <summary>
    /// The registration event args.
    /// </summary>
    public class RegistrationEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationEventArgs"/> class.
        /// </summary>
        /// <param name="serviceContainer">
        /// The service container.
        /// </param>
        public RegistrationEventArgs(IServiceContainer serviceContainer)
        {
            this.ServiceContainer = serviceContainer;
        }

        /// <summary>
        /// Gets the service container.
        /// </summary>
        public IServiceContainer ServiceContainer { get; private set; }
    }
}