// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFunctionalServiceFactory.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the IFunctionalServiceFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kimado.Core.Contracts
{
    /// <summary>
    /// The FunctionalServiceFactory interface.
    /// </summary>
    public interface IFunctionalServiceFactory
    {
        /// <summary>
        /// Registers the functional service with the factory and the delegate to create the functional service
        /// </summary>
        /// <typeparam name="TServiceType">Functional service
        /// </typeparam>
        /// <param name="createMethod">
        /// The create Method.
        /// </param>
        void Register<TServiceType>(Func<TServiceType> createMethod) where TServiceType : IFunctionalService;

        /// <summary>
        /// Looks up the concrete factory based on the ServiceType, call the constructor and return the ServiceType interface
        /// </summary>
        /// <typeparam name="TServiceType">service type</typeparam>
        /// <returns>ServiceType interface</returns>
        TServiceType Instance<TServiceType>() where TServiceType : IFunctionalService;
    }
}