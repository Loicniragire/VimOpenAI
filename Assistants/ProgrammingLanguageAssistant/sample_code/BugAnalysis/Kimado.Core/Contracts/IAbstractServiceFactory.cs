// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAbstractServiceFactory.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   The AbstractServiceFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kimado.Core.Contracts
{
    /// <summary>
    /// The AbstractServiceFactory interface.
    /// </summary>
    public interface IAbstractServiceFactory
    {
        /// <summary>
        /// Register will register abstract concrete constructors for the abstract types
        /// </summary>
        /// <typeparam name="T">T is the interface type which is registered in the factory</typeparam>
        /// <param name="createMethod">Delegate method which creates the concrete type</param>
        void Register<T>(Func<T> createMethod);

        /// <summary>
        /// Instance<typeparamref name="T"/> returns the interface for the registered abstract type
        /// </summary>
        /// <typeparam name="T">T is the interface type which is registered in the factory</typeparam>
        /// <returns>Returns an interface of the service which implements T</returns>
        T Instance<T>();
    }
}