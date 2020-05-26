// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.Confluence.Entities;
using Dapplo.Confluence.Internals;
using Dapplo.HttpExtensions;

namespace Dapplo.Confluence
{
    /// <summary>
    ///     Marker interface for the methods which have no direct clear domain
    /// </summary>
    public interface IMiscDomain : IConfluenceDomain
    {

    }

    /// <summary>
    ///     All extensions which have no direct clear domain
    /// </summary>
    public static class MiscExtensions
    {
        /// <summary>
        ///     Retrieve the picture for the supplied Picture entity
        /// </summary>
        /// <param name="confluenceClient">IMiscDomain to bind the extension method to</param>
        /// <typeparam name="TResponse">the type to return the result into. e.g. Bitmap,BitmapSource or MemoryStream</typeparam>
        /// <param name="picture">Picture from User, Space, History etc</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Bitmap,BitmapSource or MemoryStream (etc) depending on TResponse</returns>
        public static async Task<TResponse> GetPictureAsync<TResponse>(this IMiscDomain confluenceClient, Picture picture,
            CancellationToken cancellationToken = default)
            where TResponse : class
        {
            confluenceClient.Behaviour.MakeCurrent();

            var pictureUriBuilder = new UriBuilder(
                confluenceClient.ConfluenceUri.Scheme,
                confluenceClient.ConfluenceUri.Host,
                confluenceClient.ConfluenceUri.Port);
            var pictureUri = new Uri(pictureUriBuilder.Uri.AbsoluteUri.TrimEnd('/') + picture.Path);
            var response = await pictureUri.GetAsAsync<HttpResponse<TResponse, string>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors();
        }

        /// <summary>
        /// Returns the system information for the Confluence Cloud tenant.
        /// This only works on the cloud version of Confluence.
        /// </summary>
        /// <param name="confluenceClient">IMiscDomain</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>SystemInfoEntity</returns>
        public static async Task<SystemInfoEntity> GetSystemInfoAsync(this IMiscDomain confluenceClient, CancellationToken cancellationToken = default)
        {
            var systemInfoUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("settings", "systemInfo");

            // Special error handling
            var behaviour = confluenceClient.Behaviour.ShallowClone();
            behaviour.ThrowOnError = false;
            behaviour.MakeCurrent();

            var response = await systemInfoUri.GetAsAsync<HttpResponse<SystemInfoEntity>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors(HttpStatusCode.OK, HttpStatusCode.NotFound);
        }
    }
}