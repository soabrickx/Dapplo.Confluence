// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using Dapplo.HttpExtensions.OAuth;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions;

namespace Dapplo.Confluence.OAuth
{
    /// <summary>
    ///     A Confluence client with OAuth support build by using Dapplo.HttpExtensions
    /// </summary>
    public class ConfluenceOAuthClient : ConfluenceClient
    {
        private ConfluenceOAuthClient(Uri baseUri, ConfluenceOAuthSettings confluenceOAuthSettings, IHttpSettings httpSettings = null) : base(baseUri, httpSettings)
        {
            var confluenceOAuthUri = ConfluenceUri.AppendSegments("plugins", "servlet", "oauth");

            var oAuthSettings = new OAuth1Settings
            {
                TokenUrl = confluenceOAuthUri.AppendSegments("request-token"),
                TokenMethod = HttpMethod.Post,
                AccessTokenUrl = confluenceOAuthUri.AppendSegments("access-token"),
                AccessTokenMethod = HttpMethod.Post,
                CheckVerifier = false,
                SignatureType = OAuth1SignatureTypes.RsaSha1,
                // According to <a href="https://community.atlassian.com/t5/Questions/Confluence-Oauth-Authentication/qaq-p/331326#M51385">here</a>
                // the OAuth arguments need to be passed in the query
                SignatureTransport = OAuth1SignatureTransports.QueryParameters,
                Token = confluenceOAuthSettings.Token,
                ClientId = confluenceOAuthSettings.ConsumerKey,
                CloudServiceName = confluenceOAuthSettings.CloudServiceName,
                RsaSha1Provider = confluenceOAuthSettings.RsaSha1Provider,
                AuthorizeMode = confluenceOAuthSettings.AuthorizeMode,
                AuthorizationUri = confluenceOAuthUri.AppendSegments("authorize")
                    .ExtendQuery(new Dictionary<string, string>
                    {
                        {OAuth1Parameters.Token.EnumValueOf(), "{RequestToken}"},
                        {OAuth1Parameters.Callback.EnumValueOf(), "{RedirectUrl}"}
                    })
            };

            // Configure the OAuth1Settings
            Behaviour = ConfigureBehaviour(OAuth1HttpBehaviourFactory.Create(oAuthSettings), httpSettings);
        }
        /// <summary>
        ///     Create the IConfluenceClient, using OAuth 1 for the communication, here the HttpClient is configured
        /// </summary>
        /// <param name="baseUri">Base URL, e.g. https://yourconfluenceserver</param>
        /// <param name="confluenceOAuthSettings">ConfluenceOAuthSettings</param>
        /// <param name="httpSettings">IHttpSettings or null for default</param>
        public static IConfluenceClient Create(Uri baseUri, ConfluenceOAuthSettings confluenceOAuthSettings, IHttpSettings httpSettings = null)
        {
            return new ConfluenceOAuthClient(baseUri, confluenceOAuthSettings, httpSettings);
        }
    }
}