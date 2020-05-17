// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.Confluence.Entities;
using Dapplo.Confluence.Internals;
using Dapplo.HttpExtensions;

namespace Dapplo.Confluence
{
    /// <summary>
    ///     Marker interface for the user domain
    /// </summary>
    public interface IUserDomain : IConfluenceDomain
    {
    }

    /// <summary>
    ///     All user related functionality
    /// </summary>
    public static class UserDomain
    {
        /// <summary>
        ///     Get Anonymous user information, introduced with 6.6
        ///     See: https://docs.atlassian.com/confluence/REST/latest/#user-getAnonymous
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>User</returns>
        public static async Task<User> GetAnonymousUserAsync(this IUserDomain confluenceClient, CancellationToken cancellationToken = default)
        {
            var myselfUri = confluenceClient.ConfluenceApiUri.AppendSegments("user", "anonymous");
            confluenceClient.Behaviour.MakeCurrent();
            var response = await myselfUri.GetAsAsync<HttpResponse<User, Error>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors();
        }

        /// <summary>
        ///     Get current user information, introduced with 6.6
        ///     See: https://docs.atlassian.com/confluence/REST/latest/#user-getCurrent
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>User</returns>
        public static async Task<User> GetCurrentUserAsync(this IUserDomain confluenceClient, CancellationToken cancellationToken = default)
        {
            confluenceClient.Behaviour.MakeCurrent();
            var myselfUri = confluenceClient.ConfluenceApiUri.AppendSegments("user", "current");

            var response = await myselfUri.GetAsAsync<HttpResponse<User, Error>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors();
        }

        /// <summary>
        ///     Get user information, introduced with 6.6
        ///     See: https://docs.atlassian.com/confluence/REST/latest/#user-getUser
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="username">string with username</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>User</returns>
        [Obsolete("Username is deprecated, see: https://developer.atlassian.com/cloud/confluence/deprecation-notice-user-privacy-api-migration-guide/")]
        public static async Task<User> GetUserAsync(this IUserDomain confluenceClient, string username, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

            var userUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user")
                .ExtendQuery("username", username);
            confluenceClient.Behaviour.MakeCurrent();
            var response = await userUri.GetAsAsync<HttpResponse<User, Error>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors();
        }

        /// <summary>
        ///     Get user information, introduced with 6.6
        ///     See: https://docs.atlassian.com/confluence/REST/latest/#user-getUser
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="accountIdHolder">IAccountIdHolder</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>User</returns>
        public static async Task<User> GetUserAsync(this IUserDomain confluenceClient, IAccountIdHolder accountIdHolder, CancellationToken cancellationToken = default)
        {
            if (accountIdHolder == null || string.IsNullOrEmpty(accountIdHolder.AccountId)) throw new ArgumentNullException(nameof(accountIdHolder));

            var userUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user")
                .ExtendQuery("accountId", accountIdHolder.AccountId);
            confluenceClient.Behaviour.MakeCurrent();
            var response = await userUri.GetAsAsync<HttpResponse<User, Error>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors();
        }

        /// <summary>
        ///  Get groups for the specified user, introduced with 6.6
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-memberof-get
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="accountIdHolder">IAccountIdHolder</param>
        /// <param name="pagingInformation">PagingInformation</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>List with Groups</returns>
        public static async Task<IList<Group>> GetGroupMembershipsAsync(this IUserDomain confluenceClient, IAccountIdHolder accountIdHolder, PagingInformation pagingInformation = null, CancellationToken cancellationToken = default)
        {
            if (accountIdHolder == null || string.IsNullOrEmpty(accountIdHolder.AccountId)) throw new ArgumentNullException(nameof(accountIdHolder));

            pagingInformation ??= new PagingInformation
            {
                Limit = 200,
                Start = 0
            };
            var groupUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "memberof")
                .ExtendQuery(new Dictionary<string, object> {
                    {
                        "start", pagingInformation.Start
                    },
                    {
                        "limit", pagingInformation.Limit
                    },
                    {
                        "accountId", accountIdHolder.AccountId
                    }
                });
            confluenceClient.Behaviour.MakeCurrent();
            var response = await groupUri.GetAsAsync<HttpResponse<Result<Group>, Error>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors()?.Results;
        }

        /// <summary>
        ///  Get groups for the specified user, introduced with 6.6
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-memberof-get
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="accountIdHolder">IAccountIdHolder for the user (account id)</param>
        /// <param name="contentId">string with the ID for the content</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>List with Groups</returns>
        public static async Task AddContentWatcher(this IUserDomain confluenceClient, IAccountIdHolder accountIdHolder, string contentId, CancellationToken cancellationToken = default)
        {
            if (accountIdHolder == null || string.IsNullOrEmpty(accountIdHolder.AccountId)) throw new ArgumentNullException(nameof(accountIdHolder));
            if (string.IsNullOrEmpty(contentId)) throw new ArgumentNullException(nameof(contentId));

            var groupUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "watch", "content", contentId)
                .ExtendQuery("accountId", accountIdHolder.AccountId);

            confluenceClient.Behaviour.MakeCurrent();
            var response = await groupUri.PostAsync<HttpResponseWithError<Error>>(null, cancellationToken: cancellationToken).ConfigureAwait(false);
            response.HandleStatusCode();
        }
    }
}