// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataServiceFactory.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   DataServiceFactory implements the service factory
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using Kimado.Core.Contracts;

namespace Kimado.Core.Factories
{
    /// <summary>
    /// DataServiceFactory implements the service factory
    /// </summary>
    public class DataServiceFactory : IDataServiceFactory
    {
        /// <summary>
        /// Instance of factory
        /// </summary>
        private readonly AbstractServiceFactory<IDataService> serviceFactory = new AbstractServiceFactory<IDataService>();

        /// <summary>
        /// Register the functional service creation method
        /// </summary>
        /// <typeparam name="TServiceType">Service type
        /// </typeparam>
        /// <param name="createMethod">
        /// The create Method.
        /// </param>
        public void Register<TServiceType>(Func<TServiceType> createMethod) where TServiceType : IDataService
        {
            this.serviceFactory.Register(createMethod);
        }

        /// <summary>
        /// Instance returns a new instance of the functional service by calling the createMethod registered with the service
        /// </summary>
        /// <typeparam name="TServiceType">Service type.
        /// </typeparam>
        /// <returns>
        /// The <see cref="TServiceType"/>.
        /// </returns>
        public TServiceType Instance<TServiceType>() where TServiceType : IDataService
        {
            return this.serviceFactory.Instance<TServiceType>();
        }
    }
}