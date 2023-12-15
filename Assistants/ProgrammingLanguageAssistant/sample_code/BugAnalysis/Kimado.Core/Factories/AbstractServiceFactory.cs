// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractServiceFactory.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   AbstractServiceFactory<typeparam name="T">T is the concrete type of factory</typeparam>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using Kimado.Core.Contracts;

namespace Kimado.Core.Factories
{
    /// <summary>
    /// The abstract service factory.
    /// </summary>
    /// <typeparam name="T">The concrete type of factory
    /// </typeparam>
    public class AbstractServiceFactory<T> : IAbstractServiceFactory
    {
        /// <summary>
        /// factoryRegistry contains the registered factory create delegates
        /// </summary>
        private Dictionary<Type, Delegate> factoryRegistry = new Dictionary<Type, Delegate>();

        /// <summary>
        /// Registers the factory delegate with the abstract factory
        /// </summary>
        /// <param name="createMethod">
        /// The create method.
        /// </param>
        /// <typeparam name="TServiceType">Service type
        /// </typeparam>
        public void Register<TServiceType>(Func<TServiceType> createMethod)
        {
            Type type = typeof(TServiceType);
            if (this.factoryRegistry.ContainsKey(type))
            {
                throw new ArgumentException("ServiceType already exists in service registry");
            }

            this.factoryRegistry.Add(type, createMethod as Func<T>);
        }

        /// <summary>
        /// Instance<typeparamref name="TServiceType"/> returns the instance returned by the delegate method defined by the factoryRegistry interface type
        /// </summary>
        /// <typeparam name="TServiceType">ServiceType is the service type</typeparam>
        /// <returns>Returns an instance of the service registered in the factory</returns>
        public TServiceType Instance<TServiceType>()
        {
            Type type = typeof(TServiceType);
            if (!this.factoryRegistry.ContainsKey(type))
            {
                throw new ArgumentException(
                    $"The requested functional services \"{type.Name}\" does not exist in service registry");
            }

            Func<TServiceType> createMethod =
                    (Func<TServiceType>)this.factoryRegistry[type]; // TODO: Research constraint to force coersion.
            if (createMethod == null)
            {
                throw new ArgumentException("ServiceType does not have a factory delegate");
            }

            return createMethod();
        }
    }
}