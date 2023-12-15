// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecureWebClientService.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the SecureWebClientService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Kimado.Common.Contracts;
using Kimado.Common.Models;
using Kimado.Core;
using Kimado.Core.Contracts;

using log4net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kimado.Common.Services
{
    /// <summary>
    /// The secure web client service.
    /// </summary>
    public class SecureWebClientService : FunctionalServiceBase, ISecureWebClientService
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureWebClientService"/> class. 
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        public SecureWebClientService(IServiceContainer container)
            : base(container)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        /// <summary>
        /// CreateClient will create a WebClient and attach the BearerToken and URL
        /// </summary>
        /// <param name="url">The WebApi endpoint url</param>
        /// <param name="bearerToken">Bearer token obtained from the Identity service</param>
        /// <returns>Returns an instance of SecureWebClient containing the URL and bearer token</returns>
        public SecureWebClient CreateClient(string url, string bearerToken)
        {
            SecureWebClient webClient = new SecureWebClient()
                                            {
                                                Url = url,
                                                AccessToken = bearerToken,
                                                Content = string.Empty,
                                                Parameters = new Dictionary<string, string>()
                                            };
            return webClient;
        }

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
        public SecureWebClient CreateClient(string url, string bearerToken, X509Certificate2 certificate)
        {
            SecureWebClient webClient = this.CreateClient(url, bearerToken);
            webClient.Certificate = certificate;
            return webClient;
        }

        /// <summary>
        /// Invoke will POST a request to the URL in the webClient 
        /// </summary>
        /// <param name="webClient">SecureWebClient model containing all the Web call information</param>
        /// <returns>Returns the HttpResponseMessage (</returns>
        public HttpResponseMessage Invoke(SecureWebClient webClient)
        {
            try
            {
                var handler = new HttpClientHandler();
                if (webClient.Certificate != null)
                {
                    handler.ClientCertificates.Add(webClient.Certificate);
                }

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(webClient.AccessToken))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", webClient.AccessToken);
                    }

                    return client.PostAsync(
                        webClient.Url,
                        new StringContent(webClient.Content, Encoding.UTF8, "application/json")).Result;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Posts a request to the URL in the webClient 
        /// </summary>
        /// <param name="webClient">
        /// SecureWebClient model containing all the Web call information
        /// </param>
        /// <param name="httpMethod">
        /// The http Method.
        /// </param>
        /// <returns>
        /// Returns the HttpResponseMessage (
        /// </returns>
        public HttpResponseMessage Invoke(SecureWebClient webClient, HttpMethod httpMethod)
        {
            try
            {
                var handler = new HttpClientHandler();
                if (webClient.Certificate != null)
                {
                    handler.ClientCertificates.Add(webClient.Certificate);
                }

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(webClient.AccessToken))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", webClient.AccessToken);
                    }

                    if (httpMethod == HttpMethod.Post)
                    {
                        return client.PostAsync(
                            webClient.Url,
                            new StringContent(webClient.Content, Encoding.UTF8, "application/json")).Result;
                    }
                    else if (httpMethod == HttpMethod.Put)
                    {
                        return client.PutAsync(
                            webClient.Url,
                            new StringContent(webClient.Content, Encoding.UTF8, "application/json")).Result;
                    }
                    else if (httpMethod == HttpMethod.Delete)
                    {
                        return client.DeleteAsync(webClient.Url).Result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// GetResponse will issue a GET request to the url specified
        /// </summary>
        /// <param name="webClient">SecureWebClient model containing all the Web call information</param>
        /// <returns>Returns the HttpResponseMessage (</returns>
        public HttpResponseMessage GetResponse(SecureWebClient webClient)
        {
            try
            {
                var handler = new HttpClientHandler();
                if (webClient.Certificate != null)
                {
                    handler.ClientCertificates.Add(webClient.Certificate);
                }

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(webClient.AccessToken))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", webClient.AccessToken);
                    }

                    string url = webClient.Url;
                    if (webClient.Parameters.Keys.Count > 0)
                    {
                        List<string> parameters = new List<string>();
                        foreach (string key in webClient.Parameters.Keys)
                        {
                            parameters.Add($"{key}={webClient.Parameters[key]}");
                        }

                        url = url + "?" + string.Join("&", parameters);
                    }

                    return client.GetAsync(url).Result;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get an AuthToken from the Identity service
        /// </summary>
        /// <param name="clientCertificate">Client certificate
        /// </param>
        /// <param name="username">the username
        /// </param>
        /// <param name="password">the password
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// Returns the bearer token
        /// </returns>
        public AccessTokenResponse GetAccessToken(
            X509Certificate2 clientCertificate,
            string username,
            string password,
            string url)
        {
            var pairs = new List<KeyValuePair<string, string>>
                            {
                                new KeyValuePair<string, string>(
                                    "grant_type",
                                    "password"),
                                new KeyValuePair<string, string>(
                                    "Username",
                                    username),
                                new KeyValuePair<string, string>(
                                    "Password",
                                    password)
                            };
            var content = new FormUrlEncodedContent(pairs);

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(clientCertificate);
            using (var client = new HttpClient(handler))
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                var response = client.PostAsync(url, content).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                AccessTokenResponse accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(result);
                return accessTokenResponse;
            }
        }

        /// <summary>
        /// ValidateToken will validate the access token against the URL and return the resource grants
        /// </summary>
        /// <param name="clientCertificate">X509Certificate2 client certificate</param>
        /// <param name="accessToken">The Access token granted in a previous call to the Identity service</param>
        /// <param name="url">URL endpoint to the identity service</param>
        /// <returns>Returns the JSON string containing the validation information and the grants and revokes</returns>
        public string ValidateToken(X509Certificate2 clientCertificate, string accessToken, string url)
        {
            var handler = new HttpClientHandler();
            try
            {
                handler.ClientCertificates.Add(clientCertificate);
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("accept", "application/json");
                    var response = client.GetAsync(url + "/" + accessToken).Result;
                    var result = response.Content.ReadAsStringAsync().Result;
                    var authResponseObject = JObject.Parse(result);
                    return authResponseObject.ToString();
                }
            }
            catch (AggregateException ae)
            {
                foreach (Exception e in ae.InnerExceptions)
                {
                    Log.Error(e.Message);
                }

                throw;
            }
        }
    }
}
