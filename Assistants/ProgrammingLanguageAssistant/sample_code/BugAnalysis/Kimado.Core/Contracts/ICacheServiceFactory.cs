// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICacheServiceFactory.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the ICacheServiceFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kimado.Core.Contracts
{
    /// <summary>
    /// The CacheServiceFactory interface.
    /// </summary>
    public interface ICacheServiceFactory
    {
        /// <summary>
        /// Register will register the data service with the factory and the delegate to create the functional service
        /// </summary>
        /// <typeparam name="TEntityType">class which is used to type the cache.
        /// </typeparam>
        /// <param name="createMethod">
        /// The create Method.
        /// </param>
        void Register<TEntityType>(Func<ICacheService<TEntityType>> createMethod) where TEntityType : class;

        /// <summary>
        /// Instance Will retrieve the registed cache for the specified type and if it doesn't exist 
        /// as an active cache, then it will call the factory concrete constructor
        /// </summary>
        /// <typeparam name="TEntityType">class which is used to type the cache.</typeparam>
        /// <returns>Returns an instance of the instance of a CacheService for the EntityType.</returns>
        ICacheService<TEntityType> Instance<TEntityType>() where TEntityType : class;
    }
}