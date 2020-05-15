// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.Confluence.Entities;
using Dapplo.Confluence.Internals;
using Dapplo.HttpExtensions;

namespace Dapplo.Confluence
{
    /// <summary>
    ///     Marker interface for the group domain
    /// </summary>
    public interface IGroupDomain : IConfluenceDomain
    {
    }

    /// <summary>
    ///     All group related functionality
    /// </summary>
    public static class GroupDomain
    {
        /// <summary>
        ///     Get all the groups
        /// </summary>
        /// <param name="confluenceClient">IGroupDomain to bind the extension method to</param>
        /// <param name="pagingInformation">PagingInformation</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>List with Groups</returns>
        public static async Task<IList<Group>> GetGroupsAsync(this IGroupDomain confluenceClient, PagingInformation pagingInformation = null, CancellationToken cancellationToken = default)
        {
            pagingInformation ??= new PagingInformation {
                Limit = 220,
                Start = 0
            };
            var groupUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("group")
                .ExtendQuery(new Dictionary<string, object> {
                    {
                        "start", pagingInformation.Start
                    },
                    {
                        "limit", pagingInformation.Limit
                    }
                });
            confluenceClient.Behaviour.MakeCurrent();
            var response = await groupUri.GetAsAsync<HttpResponse<Result<Group>, Error>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors()?.Results;
        }
    }
}