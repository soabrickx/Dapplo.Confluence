// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


#if NET471 || NET461 || NETCOREAPP3_0
using Dapplo.HttpExtensions.OAuth;
using System.Security.Cryptography;

namespace Dapplo.Confluence
{
    /// <summary>
    /// OAuth 1 settings for Confluence Oauth connections
    /// </summary>
    public class ConfluenceOAuthSettings
    {
        /// <summary>
        /// Consumer Key which is set in the Confluence Application link
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Confluence uses OAuth1 with RSA-SHA1, for this a RSACryptoServiceProvider is used.
        /// This needs to be created from a private key, the represented public key is set in the linked-applications
        /// </summary>
        public RSACryptoServiceProvider RsaSha1Provider { get; set; }

        /// <summary>
        /// The AuthorizeMode to use
        /// </summary>
        public AuthorizeModes AuthorizeMode { get; set; } = AuthorizeModes.LocalhostServer;

        /// <summary>
        /// Name of the cloud service, which is displayed in the embedded browser / browser when using AuthorizeModes.EmbeddedBrowser
        /// </summary>
        public string CloudServiceName { get; set; } = "Confluence";

        /// <summary>
        /// The token object for storing the OAuth 1 secret etc, implement your own IOAuth1Token to be able to store these
        /// </summary>
        public IOAuth1Token Token { get; set; } = new OAuth1Token();
    }
}
#endif