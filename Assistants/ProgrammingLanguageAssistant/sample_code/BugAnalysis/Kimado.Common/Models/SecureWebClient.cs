// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecureWebClient.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the SecureWebClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Kimado.Common.Models
{
    /// <summary>
    /// The secure web client.
    /// </summary>
    public class SecureWebClient
    {
        /// <summary>
        /// Gets or sets WebApi endpoint URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets Bearer token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets Json content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets URL parameters
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// Gets or sets Client certificate
        /// </summary>
        public X509Certificate2 Certificate { get; set; }
    }
}