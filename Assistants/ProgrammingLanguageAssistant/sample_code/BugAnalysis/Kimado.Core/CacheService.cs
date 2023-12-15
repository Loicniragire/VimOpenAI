// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheService.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   CacheServiceFactory implements the cache service factory
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Kimado.Core.Contracts;

namespace Kimado.Core
{
    /// <summary>
    /// CacheServiceFactory implements the cache service factory
    /// </summary>
    /// <typeparam name="TEntityType">Cache service type
    /// </typeparam>
    public class CacheService<TEntityType> : FunctionalServiceBase, ICacheService<TEntityType>
        where TEntityType : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService{TEntityType}"/> class. 
        /// </summary>
        /// <param name="serviceEngine">
        /// IServiceContainer core
        /// </param>
        /// <param name="expirationDuration">
        /// The expiration Duration.
        /// </param>
        public CacheService(IServiceContainer serviceEngine, int expirationDuration = -1)
            : base(serviceEngine)
        {
            this.CacheElements = new Dictionary<string, CacheElement<TEntityType>>();
            this.ExpirationDuration = expirationDuration == -1
                                          ? TimeSpan.FromMinutes(30)
                                          : TimeSpan.FromSeconds(expirationDuration);
        }

        /// <summary>
        /// Gets or sets the expiration duration.
        /// </summary>
        public TimeSpan ExpirationDuration { get; set; }

        /// <summary>
        /// Gets or sets the cache elements.
        /// </summary>
        private Dictionary<string, CacheElement<TEntityType>> CacheElements { get; set; }

        /// <summary>
        /// Index property to get/set elements in the cache
        /// </summary>
        /// <param name="key">object which must support a ToString() method to use as the key</param>
        /// <returns>Returns an EntityType object which is found in the cache</returns>
        public TEntityType this[object key]
        {
            get => (key != null) ? this.Get(key) : null;

            set => this.Add(key, value);
        }

        /// <summary>
        /// Add will add an element to the cache service
        /// </summary>
        /// <param name="key">object which must support a ToString() method to use as the key</param>
        /// <param name="entity">EntityType object which is to be stored in the cache</param>
        public void Add(object key, TEntityType entity)
        {
            this.CacheElements.Add(key.ToString(), new CacheElement<TEntityType>(entity));
        }

        /// <summary>
        /// Get returns an element from the cache
        /// </summary>
        /// <param name="typeKey">
        /// The typeKey.
        /// </param>
        /// <returns>
        /// Returns an EntityType object which is found in the cache
        /// </returns>
        public TEntityType Get(object typeKey)
        {
            if (typeKey == null)
            {
                return null;
            }

            string key = typeKey.ToString();
            CacheElement<TEntityType> element;
            if (!this.CacheElements.TryGetValue(key, out element))
            {
                return null;
            }

            // Check to see if element has expired and remove it from the cache
            if (element.Created + this.ExpirationDuration >= DateTime.Now)
            {
                return element.Value;
            }

            this.Remove(typeKey);
            return null;
        }

        /// <summary>
        /// Remove an entry from the cache even if its not expired.
        /// </summary>
        /// <param name="typeKey">
        /// The type Key.
        /// </param>
        public void Remove(object typeKey)
        {
            string key = typeKey?.ToString();
            if (!string.IsNullOrEmpty(key))
            {
                lock (this.CacheElements)
                {
                    if (this.CacheElements.ContainsKey(key))
                    {
                        this.CacheElements.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Cleanup will walk the cache and remove any expired elements
        /// </summary>
        public void Cleanup()
        {
            List<string> allKeys = this.CacheElements.Keys.ToList();
            foreach (string key in allKeys)
            {
                // Performing a get will remove an element if its expired
                this.Get(key);
            }
        }
    }
}