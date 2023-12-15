// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceConfiguration.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the IServiceConfiguration type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kimado.Core.Contracts
{
    /// <summary>
    /// The ServiceConfiguration interface.
    /// </summary>
    public interface IServiceConfiguration
    {
        /// <summary>
        /// Initialize the IOC container
        /// </summary>
        /// <param name="container">IServiceContainer container</param>
        void Initialize(IServiceContainer container);
    }
}