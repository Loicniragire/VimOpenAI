// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISecureWebClientService.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the ISecureWebClientService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

using Kimado.Common.Models;
using Kimado.Core.Contracts;

namespace Kimado.Common.Contracts
{
    /// <summary>
    /// The SecureWebClientService interface.
    /// </summary>
    public interface ISecureWebClientService : IFunctionalService
    {
        /// <summary>
        /// Gets an AuthToken from the Identity service
        /// </summary>
        /// <param name="clientCertificate">Client certificate
        /// </param>
        /// <param name="username">user name
        /// </param>
        /// <param name="password">the password
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// Returns the bearer token
        /// </returns>
        AccessTokenResponse GetAccessToken(X509Certificate2 clientCertificate, string username, string password, string url);

        /// <summary>
        /// Validates the access token against the URL and return the resource grants
        /// </summary>
        /// <param name="clientCertificate">Client certificate</param>
        /// <param name="accessToken">access token</param>
        /// <param name="url">access token Url</param>
        /// <returns>Resource grant</returns>
        string ValidateToken(X509Certificate2 clientCertificate, string accessToken, string url);

        /// <summary>
        /// CreateClient will create a WebClient and attach the BearerToken and URL.
        /// </summary>
        /// <param name="url">The WebApi endpoint url</param>
        /// <param name="bearerToken">Bearer token obtained from the Identity service</param>
        /// <returns>Returns a model that contains the web client parameters</returns>
        SecureWebClient CreateClient(string url, string bearerToken);

        /// <summary>
        /// CreateClient will create a WebClient and attach the BearerToken and URL.
        /// </summary>
        /// <param name="url">
        /// The WebApi endpoint url
        /// </param>
        /// <param name="bearerToken">
        /// Bearer token obtained from the Identity service
        /// </param>
        /// <param name="certificate">
        /// The certificate.
        /// </param>
        /// <returns>
        /// Returns a model that contains the web client parameters
        /// </returns>
        SecureWebClient CreateClient(string url, string bearerToken, X509Certificate2 certificate);

        /// <summary>
        /// Invoke will POST a request to the 
        /// </summary>
        /// <param name="webClient">SecureWebClient model containing all the Web call information</param>
        /// <returns>Returns the HttpResponseMessage (</returns>
        HttpResponseMessage Invoke(SecureWebClient webClient);

        /// <summary>
        /// Invoke will POST or PUT a request to the URL in the webClient 
        /// </summary>
        /// <param name="webClient">SecureWebClient model containing all the Web call information</param>
        /// <param name="httpMethod">HttpMethod used for HttpClient.  Methods supported are HttpMethod.Post, 
		/// HttpMethod.Put, and HttpMethod.Delete.  All other method types return null.</param>
        /// <returns>Returns the HttpResponseMessage (</returns>
        HttpResponseMessage Invoke(SecureWebClient webClient, HttpMethod httpMethod);

        /// <summary>
        /// GetResponse will issue a GET request to the url specified
        /// </summary>
        /// <param name="webClient">SecureWebClient model containing all the Web call information</param>
        /// <returns>Returns the HttpResponseMessage (</returns>
        HttpResponseMessage GetResponse(SecureWebClient webClient);
    }
}
