// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheServiceFactory.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   CacheServiceFactory implements the cache service factory
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

using Kimado.Core.Contracts;

namespace Kimado.Core.Factories
{
    /// <summary>
    /// CacheServiceFactory implements the cache service factory
    /// </summary>
    public class CacheServiceFactory : ICacheServiceFactory
    {
        /// <summary>
        /// The factory registry.
        /// </summary>
        private readonly Dictionary<Type, Delegate> factoryRegistry = new Dictionary<Type, Delegate>();

        /// <summary>
        /// The cache registry.
        /// </summary>
        private readonly Dictionary<Type, object> cacheRegistry = new Dictionary<Type, object>();



        /// <summary>
        /// Initializes a new instance of the <see cref="CacheServiceFactory"/> class. 
        /// Must construct the factory with the service engine
        /// </summary>
        /// <param name="serviceEngine">
        /// Service Container
        /// </param>
        public CacheServiceFactory(IServiceContainer serviceEngine)
        {
            this.ServiceEngine = serviceEngine;

            // Start the thread to perform cache cleanup
            Thread cleanupThread = new Thread(this.CleanupThread);
            cleanupThread.Start();
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="CacheServiceFactory"/> class from being created.
        /// </summary>
        private CacheServiceFactory()
        {
        }

        /// <summary>
        /// Gets or sets the service engine. This is the service engine in which this CacheServiceFactory is contained
        /// </summary>
        private IServiceContainer ServiceEngine { get; set; }

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="createMethod">
        /// The create method.
        /// </param>
        /// <typeparam name="TEntityType">Cache service.
        /// </typeparam>
        public void Register<TEntityType>(Func<ICacheService<TEntityType>> createMethod)
            where TEntityType : class
        {
            Type type = typeof(ICacheService<TEntityType>);
            if (this.factoryRegistry.ContainsKey(type))
            {
                throw new ArgumentException("EntityType already exists in CacheServiceFactory registry");
            }

            this.factoryRegistry.Add(type, createMethod);
        }

        /// <summary>
        /// Returns a new instance of the functional service by calling the createMethod registered with the service
        /// </summary>
        /// <typeparam name="TEntityType">Cache service
        /// </typeparam>
        /// <returns>
        /// The Cache service
        /// </returns>
        public ICacheService<TEntityType> Instance<TEntityType>()
            where TEntityType : class
        {
            Type type = typeof(ICacheService<TEntityType>);
            if (!this.factoryRegistry.ContainsKey(type))
            {
                throw new ArgumentException(
                    $"The concrete factory for '{type.Name}' has not been registered in the CacheServiceFactory registry!");
            }

            // If the cacheRegistry doesn't already contain an instance of a cache for this type, create one
            if (!this.cacheRegistry.ContainsKey(type))
            {
                Func<ICacheService<TEntityType>> createMethod = (Func<ICacheService<TEntityType>>)this.factoryRegistry[type];
                if (createMethod == null)
                {
                    throw new ArgumentException(
                        $"The registered factory '{type.Name}' does not have a concrete factory delegate!");
                }

                this.cacheRegistry.Add(type, createMethod());
            }

            return (ICacheService<TEntityType>)this.cacheRegistry[type];
        }

        /// <summary>
        /// CleanupThread is started when the factory is created
        /// </summary>
        private void CleanupThread()
        {
            Thread.CurrentThread.IsBackground = true;
            while (true)
            {
                try
                {
                    List<object> registry = this.cacheRegistry.Values.ToList();
                    foreach (var cache in registry)
                    {
                        Type t = cache.GetType();
                        MethodInfo fn = t.GetMethod("Cleanup");
                        fn?.Invoke(cache, BindingFlags.Default, null, null, null);
                    }
                }
                catch
                {
                    // ignore
                }

                Thread.Sleep(30 * 60 * 1000); // Sleep for 30 minutes
            }
        }
    }
}