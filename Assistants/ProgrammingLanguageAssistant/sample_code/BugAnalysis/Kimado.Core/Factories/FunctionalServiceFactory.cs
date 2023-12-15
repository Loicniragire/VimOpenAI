// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionalServiceFactory.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   FunctionalServiceFactory implements the service factory
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using Kimado.Core.Contracts;

namespace Kimado.Core.Factories
{
    /// <summary>
    /// FunctionalServiceFactory implements the service factory
    /// </summary>
    public class FunctionalServiceFactory : IFunctionalServiceFactory
    {
        /// <summary>
        /// Instance of factory
        /// </summary>
        private readonly AbstractServiceFactory<IFunctionalService> serviceFactory = new AbstractServiceFactory<IFunctionalService>();

        /// <summary>
        /// Register the functional service creation method
        /// </summary>
        /// <typeparam name="TServiceType">Service type.
        /// </typeparam>
        /// <param name="createMethod">
        /// The create Method.
        /// </param>
        public void Register<TServiceType>(Func<TServiceType> createMethod) where TServiceType : IFunctionalService
        {
            this.serviceFactory.Register<TServiceType>(createMethod);
        }

        /// <summary>
        /// Instance returns a new instance of the functional service by calling the createMethod registered with the service
        /// </summary>
        /// <typeparam name="TServiceType">Service type
        /// </typeparam>
        /// <returns>
        /// The <see cref="TServiceType"/>.
        /// </returns>
        public TServiceType Instance<TServiceType>() where TServiceType : IFunctionalService
        {
            return this.serviceFactory.Instance<TServiceType>();
        }
        
        /// <summary>
        /// Instance returns a new instance of the functional service by calling the createMethod registered with the service
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public TServiceType Instance<TServiceType>(params object[] arguments) where TServiceType : IFunctionalService
        {
            return this.serviceFactory.Instance<TServiceType>();
        }
    }
}