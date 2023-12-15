// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFunctionalServiceBase.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the IFunctionalServiceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kimado.Core.Contracts
{
    /// <summary>
    /// The FunctionalServiceBase interface.
    /// </summary>
    public interface IFunctionalServiceBase : IFunctionalService
    {
        /// <summary>
        /// Gets the service engine interface
        /// </summary>
        IServiceContainer ServiceContainer { get; }

        /// <summary>
        /// Gets the context ID from the service engine
        /// </summary>
        Guid ContextId { get; }
    }
}