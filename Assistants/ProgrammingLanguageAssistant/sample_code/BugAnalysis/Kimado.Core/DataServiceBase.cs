// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataServiceBase.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the DataServiceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kimado.Core.Contracts;

namespace Kimado.Core
{
    /// <summary>
    /// The data service base.
    /// </summary>
    public class DataServiceBase : IDataServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceBase"/> class. 
        /// </summary>
        /// <param name="serviceContainer">
        /// The service Container.
        /// </param>
        public DataServiceBase(IServiceContainer serviceContainer)
        {
            this.ServiceContainer = serviceContainer;
        }

        /// <summary>
        /// Gets the service container. Keeps track of the service engine interface
        /// </summary>
        public IServiceContainer ServiceContainer { get; private set; }
    }
}