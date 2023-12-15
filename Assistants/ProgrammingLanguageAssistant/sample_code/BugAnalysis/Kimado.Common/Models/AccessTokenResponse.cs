// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessTokenResponse.cs" company="Kimado">
//   Property of Kimado
// </copyright>
// <summary>
//   Defines the AccessTokenResponse type.
// Access Token response returns the deserialized token data
//
//  "access_token": "kd6YPt730ueDhoaYDo6fXmPeoOBifBSM",
//  "token_type": "Bearer",
//  "expires_in": 28800,
//  "userName": "Public_088637e3-f718-4f04-adbc-0e54a7f42aca",
//  ".issued": "Wed, 01 Aug 2018 01:48:55 GMT",
//  ".expires": "Wed, 01 Aug 2018 09:48:55 GMT"
//
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using Newtonsoft.Json;

namespace Kimado.Common.Models
{
    /// <summary>
    /// The access token response.
    /// </summary>
    public class AccessTokenResponse
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        [JsonProperty(PropertyName = "access_token")]
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the token type.
        /// </summary>
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Gets or sets the expires in.
        /// </summary>
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the issued.
        /// </summary>
        [JsonProperty(PropertyName = ".issued")]
        public DateTime Issued { get; set; }

        /// <summary>
        /// Gets or sets the expires.
        /// </summary>
        [JsonProperty(PropertyName = ".expires")]
        public DateTime Expires { get; set; }
    }
}