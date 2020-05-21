// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Threading;
using System.Threading.Tasks;
#if NET471 || NET461 || NETCOREAPP3_0
using Dapplo.HttpExtensions.OAuth;
using System.Collections.Generic;
using System.Net.Http;
using Dapplo.HttpExtensions.Extensions;
#endif
#if NET471 || NET461
using System.Net.Cache;
#endif

using Dapplo.Confluence.Entities;
using Dapplo.HttpExtensions;
using Dapplo.HttpExtensions.JsonNet;

namespace Dapplo.Confluence
{
    /// <summary>
    ///     A Confluence client build by using Dapplo.HttpExtensions
    /// </summary>
    public class ConfluenceClient : IConfluenceClientPlugins, IAttachmentDomain, IUserDomain, ISpaceDomain, IContentDomain, IMiscDomain, IGroupDomain
    {
        private bool? _isCloudServer;

        /// <summary>
        ///     Password for the basic authentication
        /// </summary>
        private string _password;

        /// <summary>
        ///     User for the basic authentication
        /// </summary>
        private string _user;

        /// <summary>
        ///     Create the ConfluenceApi object, here the HttpClient is configured
        /// </summary>
        /// <param name="confluenceUri">Base URL, e.g. https://yourConfluenceserver</param>
        /// <param name="httpSettings">IHttpSettings or null for default</param>
        private ConfluenceClient(Uri confluenceUri, IHttpSettings httpSettings = null)
        {
            ConfluenceUri = confluenceUri ?? throw new ArgumentNullException(nameof(confluenceUri));
            ConfluenceApiUri = confluenceUri.AppendSegments("rest", "api");

            Behaviour = ConfigureBehaviour(new HttpBehaviour(), httpSettings);
        }

        /// <summary>
        ///     The IHttpBehaviour for this Confluence instance
        /// </summary>
        public IHttpBehaviour HttpBehaviour => Behaviour;

        /// <summary>
        ///     Store the specific HttpBehaviour, which contains a IHttpSettings and also some additional logic for making a
        ///     HttpClient which works with Confluence
        /// </summary>
        public IHttpBehaviour Behaviour { get; protected set; }

        /// <summary>
        ///     Plugins dock to this property by implementing an extension method to IConfluenceClientPlugins
        /// </summary>
        public IConfluenceClientPlugins Plugins => this;

        /// <summary>
        ///     Set Basic Authentication for the current client
        /// </summary>
        /// <param name="user">username</param>
        /// <param name="password">password</param>
        public void SetBasicAuthentication(string user, string password)
        {
            _user = user;
            _password = password;
        }

        /// <summary>
        ///     The base URI for your Confluence server api calls
        /// </summary>
        public Uri ConfluenceApiUri { get; }

        /// <summary>
        ///     The base URI for your Confluence server downloads
        /// </summary>
        public Uri ConfluenceUri { get; }

        /// <inheritdoc />
        public IAttachmentDomain Attachment => this;

        /// <inheritdoc />
        public IContentDomain Content => this;

        /// <inheritdoc />
        public IGroupDomain Group => this;

        /// <inheritdoc />
        public IMiscDomain Misc => this;

        /// <inheritdoc />
        public ISpaceDomain Space => this;

        /// <inheritdoc />
        public IUserDomain User => this;

        /// <inheritdoc />
        public Uri CreateWebUiUri(Links links)
        {
            if (links == null)
            {
                throw new ArgumentNullException(nameof(links));
            }
            if (links.Base == null)
            {
                links.Base = ConfluenceUri;
            }
            return Concat(links.Base, links.WebUi);
        }

        /// <inheritdoc />
        public Uri CreateTinyUiUri(Links links)
        {
            if (links == null)
            {
                throw new ArgumentNullException(nameof(links));
            }
            if (links.Base == null)
            {
                links.Base = ConfluenceUri;
            }
            return Concat(links.Base, links.TinyUi);
        }

        /// <inheritdoc />
        public Uri CreateDownloadUri(Links links)
        {
            if (links == null)
            {
                throw new ArgumentNullException(nameof(links));
            }
            if (links.Base == null)
            {
                links.Base = ConfluenceUri;
            }
            return Concat(links.Base, links.Download);
        }

        /// <summary>
        ///     Helper method to combine an Uri with a path including optional query
        /// </summary>
        /// <param name="baseUri">Uri for the base</param>
        /// <param name="pathWithQuery">Path and optional query</param>
        /// <returns>Uri</returns>
        private Uri Concat(Uri baseUri, string pathWithQuery)
        {
            if (baseUri == null)
            {
                throw new ArgumentNullException(nameof(baseUri));
            }
            if (string.IsNullOrEmpty(pathWithQuery))
            {
                return null;
            }

            var queryStart = pathWithQuery.IndexOf('?');
            var path = queryStart >= 0 ? pathWithQuery.Substring(0, queryStart) : pathWithQuery;
            var query = queryStart >= 0 ? pathWithQuery.Substring(queryStart + 1) : null;
            // Use the given path, without changing encoding, as it's already correctly encoded by atlassian!
            var uriBuilder = new UriBuilder(baseUri.AppendSegments(s => s, path))
            {
                Query = query ?? string.Empty
            };
            return uriBuilder.Uri;
        }

        /// <summary>
        ///     Helper method to configure the IChangeableHttpBehaviour
        /// </summary>
        /// <param name="behaviour">IChangeableHttpBehaviour</param>
        /// <param name="httpSettings">IHttpSettings</param>
        /// <returns>the behaviour, but configured as IHttpBehaviour </returns>
        protected IHttpBehaviour ConfigureBehaviour(IChangeableHttpBehaviour behaviour, IHttpSettings httpSettings = null)
        {
            behaviour.HttpSettings = httpSettings ?? HttpExtensionsGlobals.HttpSettings;
#if NET471 || NET461
            // Disable caching, if no HTTP settings were provided.
            // This is needed as was detected here: https://github.com/dapplo/Dapplo.Confluence/issues/11
            if (httpSettings == null)
            {
                behaviour.HttpSettings.RequestCacheLevel = RequestCacheLevel.NoCacheNoStore;
            }
#endif
            // Using our own Json Serializer, implemented with JsonNetJsonSerializer
            behaviour.JsonSerializer = CreateJsonNetJsonSerializer();

            behaviour.OnHttpRequestMessageCreated = httpMessage =>
            {
                httpMessage?.Headers.TryAddWithoutValidation("X-Atlassian-Token", "no-check");
                if (!string.IsNullOrEmpty(_user) && _password != null)
                {
                    httpMessage?.SetBasicAuthorization(_user, _password);
                }
                return httpMessage;
            };
            return behaviour;
        }

        /// <summary>
        /// Checks if the client is connected to a cloud server
        /// </summary>
        /// <returns>bool</returns>
        public Task<bool> IsCloudServer(CancellationToken cancellationToken = default)
        {
            if (_isCloudServer.HasValue)
            {
                return Task.FromResult(_isCloudServer.Value);
            }
            return Task.Run(async () => {
                var systemInfo = await this.Misc.GetSystemInfoAsync(cancellationToken);
                _isCloudServer =!string.IsNullOrEmpty(systemInfo?.CloudId);
                return _isCloudServer.Value;
            }, cancellationToken);
            
        }
         
        /// <summary>
        /// Factory for CreateJsonNetJsonSerializer
        /// </summary>
        /// <returns>JsonNetJsonSerializer</returns>
        public static JsonNetJsonSerializer CreateJsonNetJsonSerializer()
        {
            var result = new JsonNetJsonSerializer();
            // This should fix https://github.com/dapplo/Dapplo.Confluence/issues/41
            result.Settings.DateFormatString = result.Settings.DateFormatString.Replace("FFFFFF", "ff");
            return result;
        }

#if NET471 || NET461 ||NETCOREAPP3_0
        /// <summary>
        ///     Create the IConfluenceClient, using OAuth 1 for the communication, here the HttpClient is configured
        /// </summary>
        /// <param name="baseUri">Base URL, e.g. https://yourconfluenceserver</param>
        /// <param name="confluenceOAuthSettings">ConfluenceOAuthSettings</param>
        /// <param name="httpSettings">IHttpSettings or null for default</param>
        public static IConfluenceClient Create(Uri baseUri, ConfluenceOAuthSettings confluenceOAuthSettings, IHttpSettings httpSettings = null)
        {
            var client = new ConfluenceClient(baseUri, httpSettings);
            var confluenceOAuthUri = client.ConfluenceUri.AppendSegments("plugins", "servlet", "oauth");

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
            client.Behaviour = client.ConfigureBehaviour(OAuth1HttpBehaviourFactory.Create(oAuthSettings), httpSettings);
            return client;
        }
#endif

        /// <summary>
        ///     Factory method to create a ConfluenceClient
        /// </summary>
        /// <param name="confluenceUri">Uri to your confluence server</param>
        /// <param name="httpSettings">IHttpSettings used if you need specific settings</param>
        /// <returns>IConfluenceClient</returns>
        public static IConfluenceClient Create(Uri confluenceUri, IHttpSettings httpSettings = null)
        {
            return new ConfluenceClient(confluenceUri, httpSettings);
        }

        /// <summary>
        ///     This makes sure that the HttpBehavior is promoted for the following Http call.
        /// </summary>
        public void PromoteContext()
        {
            Behaviour.MakeCurrent();
        }
    }
}