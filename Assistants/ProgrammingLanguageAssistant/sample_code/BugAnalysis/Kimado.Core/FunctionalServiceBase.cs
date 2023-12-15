// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionalServiceBase.cs" company="Kimado">
//   Property of Fiduciary Exchange
// </copyright>
// <summary>
//   The functional service base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using Kimado.Core.Contracts;

namespace Kimado.Core
{
    /// <summary>
    /// The functional service base.
    /// </summary>
    public class FunctionalServiceBase : IFunctionalServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalServiceBase"/> class. Creates the base object and save the engine
        /// </summary>
        /// <param name="serviceContainer">
        /// The service container.
        /// </param>
        public FunctionalServiceBase(IServiceContainer serviceContainer)
        {
            this.ServiceContainer = serviceContainer;
        }

        /// <summary>
        /// Gets the service container. Keep track of the service engine interface
        /// </summary>
        public IServiceContainer ServiceContainer { get; private set; }

        /// <summary>
        /// Gets the context id. Return the context ID from the service engine
        /// </summary>
        public Guid ContextId => this.ServiceContainer.ContextId;
    }    
}