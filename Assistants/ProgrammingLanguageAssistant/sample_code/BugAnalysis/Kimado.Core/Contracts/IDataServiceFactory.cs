// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataServiceFactory.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the IDataServiceFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kimado.Core.Contracts
{
    /// <summary>
    /// The DataServiceFactory interface.
    /// </summary>
    public interface IDataServiceFactory
    {
        /// <summary>
        /// Register will register the data service with the factory and the delegate to create the functional service
        /// </summary>
        /// <typeparam name="TServiceType">IDataService type
        /// </typeparam>
        /// <param name="createMethod">
        /// The create Method.
        /// </param>
        void Register<TServiceType>(Func<TServiceType> createMethod) where TServiceType : IDataService;

        /// <summary>
        /// Create will lookup the concrete factory based on the ServiceType, call the constructor and return the ServiceType interface
        /// </summary>
        /// <typeparam name="TServiceType">ServiceType must be IDataService</typeparam>
        /// <returns>Returns an instance of the registered Data service created by the concrete factory.</returns>
        TServiceType Instance<TServiceType>() where TServiceType : IDataService;
    }
}