// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataServiceBase.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the IDataServiceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kimado.Core.Contracts
{
    /// <summary>
    /// The DataServiceBase interface.
    /// </summary>
    public interface IDataServiceBase : IDataService
    {
        /// <summary>
        /// Gets the service container
        /// </summary>
        IServiceContainer ServiceContainer { get; }
    }
}