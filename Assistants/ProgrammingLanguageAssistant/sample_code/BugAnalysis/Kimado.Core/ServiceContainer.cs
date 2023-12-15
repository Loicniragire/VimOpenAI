// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceContainer.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   ServiceContainer represents an engine instance
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Kimado.Core.Contracts;
using Kimado.Core.Factories;

namespace Kimado.Core
{
    /// <summary>
    /// ServiceContainer represents an engine instance
    /// </summary>
    public class ServiceContainer : IServiceContainer
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        private static IServiceContainer serviceContainer;

        /// <summary>
        /// Prevents a default instance of the <see cref="ServiceContainer"/> class from being created.
        /// </summary>
        private ServiceContainer()
        {
            this.FunctionalServices = new FunctionalServiceFactory();
            this.DataServices = new DataServiceFactory();
            this.CacheServices = new CacheServiceFactory(this);
            this.ContextId = Guid.Empty;
        }

        /// <summary>
        /// Event fired after registration so that non-mainline code can access the factory registries
        /// </summary>
        public event EventHandler<RegistrationEventArgs> PostRegistration;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static IServiceContainer Instance
        {
            get
            {
                if (serviceContainer == null)
                {
                    lock (typeof(ServiceContainer))
                    {
                        if (serviceContainer == null)
                        {
                            serviceContainer = new ServiceContainer();
                            serviceContainer.RegisterServices = container =>
                                throw new InvalidOperationException(
                                    "Cannot initialize the ServiceContainer: Missing ServiceContainer.Initialize(IServiceContainer) delegate method definition.");

                            // Set the default context ID
                            (serviceContainer as ServiceContainer).ContextId = Guid.Empty;
                        }
                    }
                }

                return serviceContainer;
            }
        }

        /// <summary>
        /// Gets the data services.
        /// </summary>
        public IDataServiceFactory DataServices { get; private set; }

        /// <summary>
        /// Gets the functional services.
        /// </summary>
        public IFunctionalServiceFactory FunctionalServices { get; private set; }

        /// <summary>
        /// Gets the cache services.
        /// </summary>
        public ICacheServiceFactory CacheServices { get; private set; }

        /// <summary>
        /// Gets or sets the context id.
        /// </summary>
        public Guid ContextId { get; set; }

        /// <summary>
        /// Gets or sets the register services. Its purpose is to register
        /// the functional services with the engine.
        /// </summary>
        public Action<IServiceContainer> RegisterServices { get; set; }
        public Action<IServiceContainer> RegisterInstance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether initialized.
        /// </summary>
        public bool Initialized { get; set; }

        /// <summary>
        /// Gets the container.
        /// </summary>
        public IServiceContainer Container => Instance;

        /// <summary>
        /// Initialize will call the registration actions, then initialize the instance
        /// </summary>
        /// <returns>Returns true if initialization succeeded</returns>
        public bool Initialize()
        {
            Action<IServiceContainer> registerAction = this.RegisterServices;
            if (registerAction != null)
            {
                registerAction(this);
                this.Initialized = true;
            }

            EventHandler<RegistrationEventArgs> postRegistration = this.PostRegistration;
            postRegistration?.Invoke(this, new RegistrationEventArgs(this));
            return this.Initialized;
        }

        /// <summary>
        /// The auto register.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        public void AutoRegister(string prefix = null)
        {
            this.Register<IFunctionalService>(prefix);
            this.Register<IDataService>(prefix);
        }

        /// <summary>
        /// The register as proxy.
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <typeparam name="T">The interface type which must implement IFunctionalService or IDataService
        /// </typeparam>
        public void RegisterAsProxy<T>(string prefix = null)
        {
            throw new NotImplementedException(
                "RegisterAsProxy has not been implemented.  All services must be instantiated locally.");
        }

        /// <summary>
        /// RegisterAsInstance gets all types and registers the concrete constructors
        /// </summary>
        /// <param name="prefix">
        /// The prefix.
        /// </param>
        /// <typeparam name="T">
        /// The interface type which must implement IFunctionalService or IDataService
        /// </typeparam>
        public void Register<T>(string prefix = null)
        {
            // Load all the assemblies into the AppDomain
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();
            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase))
                .ToList();
            toLoad.ForEach(
                path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

            // Reflect the AppDomain and get all types and register those with the type of T
            AppDomain app = AppDomain.CurrentDomain;
            List<Assembly> assemblies = app.GetAssemblies().ToList();
            Type[] types;
            Type targetType = typeof(T);

            foreach (Assembly a in assemblies)
            {
                if (prefix != null && a.FullName.ToLower().StartsWith(prefix.ToLower()))
                {
                    types = a.GetTypes();
                    foreach (Type t in types)
                    {
                        if (t.IsInterface)
                        {
                            continue;
                        }

                        if (t.IsAbstract)
                        {
                            continue;
                        }

                        foreach (Type iface in t.GetInterfaces())
                        {
                            if (!iface.Equals(targetType))
                            {
                                continue;
                            }

                            if (targetType.Equals(typeof(IFunctionalService)))
                            {
                                // this.FunctionalServices.Register<IFunctionalService>(createMethod);
                            }

                            //yield return (T)Activator.CreateInstance(t);
                            break;
                        }
                    }
                }
            }

            if (typeof(T) == typeof(IDataService))
            {
            }
        }
    }
}