// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICacheService.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   The CacheService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kimado.Core.Contracts
{
    /// <summary>
    /// The CacheService interface.
    /// </summary>
    /// <typeparam name="TEntityType">cache service
    /// </typeparam>
    public interface ICacheService<TEntityType> where TEntityType : class
    {
        /// <summary>
        /// Gets or sets the time the cache elements expire
        /// </summary>
        TimeSpan ExpirationDuration { get; set; }

        /// <summary>
        /// Index property to get/set elements in the cache
        /// </summary>
        /// <param name="key">object which must support a ToString() method to use as the typeKey</param>
        /// <returns>Returns an EntityType object which is found in the cache</returns>
        TEntityType this[object key] { get; set; }

        /// <summary>
        /// Add will add an element to the cache service
        /// </summary>
        /// <param name="key">object which must support a ToString() method to use as the typeKey</param>
        /// <param name="entity">EntityType object which is to be stored in the cache</param>
        void Add(object key, TEntityType entity);

        /// <summary>
        /// Get returns an element from the cache
        /// </summary>
        /// <param name="typeKey">object which must support a ToString() method to use as the typeKey</param>
        /// <returns>Returns an EntityType object which is found in the cache</returns>
        TEntityType Get(object typeKey);

        /// <summary>
        /// Remove an entry from the cache even if its not expired.
        /// </summary>
        /// <param name="typeKey">object which must support a ToString() method to use as the typeKey</param>
        void Remove(object typeKey);

        /// <summary>
        /// Cleanup will walk the cache and remove any expired elements
        /// </summary>
        void Cleanup();
    }
}