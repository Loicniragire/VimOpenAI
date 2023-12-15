// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheElement.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   CacheElement container contains the entity
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kimado.Core
{
    /// <summary>
    /// CacheElement container contains the entity
    /// </summary>
    /// <typeparam name="TEntityType">Cache element type
    /// </typeparam>
    internal class CacheElement<TEntityType>
        where TEntityType : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheElement{TEntityType}"/> class.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        public CacheElement(TEntityType element)
        {
            this.Value = element;
            this.Created = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public TEntityType Value { get; set; }
    }
}