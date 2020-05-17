// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
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
        ///  Add the user to the list of users watching the specified content
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-content-contentId-post
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="contentId">long with the ID for the content</param>
        /// <param name="accountIdHolder">IAccountIdHolder for the user (account id), null for the current user</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public static async Task AddContentWatcher(this IUserDomain confluenceClient, long contentId, IAccountIdHolder accountIdHolder = null, CancellationToken cancellationToken = default)
        {
            if (contentId == 0) throw new ArgumentNullException(nameof(contentId));

            var userWatchContentUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "watch", "content", contentId);

            // If there is no specified accountId, the current user is used
            if (accountIdHolder != null)
            {
                userWatchContentUri = userWatchContentUri.ExtendQuery("accountId", accountIdHolder.AccountId);
            }

            confluenceClient.Behaviour.MakeCurrent();
            var response = await userWatchContentUri.PostAsync<HttpResponseWithError<Error>>(null, cancellationToken: cancellationToken).ConfigureAwait(false);
            
            // Expect a 204, which is NoContent
            response.HandleStatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  Delete the user from the list of users watching the specified content
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-content-contentId-delete
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="contentId">long with the ID for the content</param>
        /// <param name="accountIdHolder">IAccountIdHolder for the user (account id), null for the current user</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public static async Task DeleteContentWatcher(this IUserDomain confluenceClient, long contentId, IAccountIdHolder accountIdHolder = null, CancellationToken cancellationToken = default)
        {
            if (contentId == 0) throw new ArgumentNullException(nameof(contentId));

            var userWatchContentUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "watch", "content", contentId);

            // If there is no specified accountId, the current user is used
            if (accountIdHolder != null)
            {
                userWatchContentUri = userWatchContentUri.ExtendQuery("accountId", accountIdHolder.AccountId);
            }

            confluenceClient.Behaviour.MakeCurrent();
            var response = await userWatchContentUri.DeleteAsync<HttpResponseWithError<Error>>(cancellationToken).ConfigureAwait(false);

            // Expect a 204, which is NoContent
            response.HandleStatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  Check if the user is watching the specified content
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-content-contentId-get
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="contentId">long with the ID for the content</param>
        /// <param name="accountIdHolder">IAccountIdHolder for the user (account id), null for the current user</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>bool</returns>
        public static async Task<bool> IsContentWatcher(this IUserDomain confluenceClient, long contentId, IAccountIdHolder accountIdHolder = null, CancellationToken cancellationToken = default)
        {
            if (contentId == 0) throw new ArgumentNullException(nameof(contentId));

            var userWatchContentUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "watch", "content", contentId);

            // If there is no specified accountId, the current user is used
            if (accountIdHolder != null)
            {
                userWatchContentUri = userWatchContentUri.ExtendQuery("accountId", accountIdHolder.AccountId);
            }

            confluenceClient.Behaviour.MakeCurrent();
            var response = await userWatchContentUri.GetAsAsync<HttpResponse<UserWatch>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors().IsWatching;
        }

        /// <summary>
        ///  Add the user to the list of users watching the specified label
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-label-labelName-post
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="label">string with the label</param>
        /// <param name="accountIdHolder">IAccountIdHolder for the user (account id), null for the current user</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public static async Task AddLabelWatcher(this IUserDomain confluenceClient, string label, IAccountIdHolder accountIdHolder = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(label)) throw new ArgumentNullException(nameof(label));

            var userWatchLabelUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "watch", "label", label);

            // If there is no specified accountId, the current user is used
            if (accountIdHolder != null)
            {
                userWatchLabelUri = userWatchLabelUri.ExtendQuery("accountId", accountIdHolder.AccountId);
            }

            confluenceClient.Behaviour.MakeCurrent();
            var response = await userWatchLabelUri.PostAsync<HttpResponseWithError<Error>>(null, cancellationToken: cancellationToken).ConfigureAwait(false);

            // Expect a 204, which is NoContent
            response.HandleStatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  Remove the user from the list of users watching the specified label
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-label-labelName-delete
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="label">string with the label</param>
        /// <param name="accountIdHolder">IAccountIdHolder for the user (account id), null for the current user</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public static async Task DeleteLabelWatcher(this IUserDomain confluenceClient, string label, IAccountIdHolder accountIdHolder = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(label)) throw new ArgumentNullException(nameof(label));

            var userWatchLabelUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "watch", "label", label);

            // If there is no specified accountId, the current user is used
            if (accountIdHolder != null)
            {
                userWatchLabelUri = userWatchLabelUri.ExtendQuery("accountId", accountIdHolder.AccountId);
            }

            confluenceClient.Behaviour.MakeCurrent();
            var response = await userWatchLabelUri.DeleteAsync<HttpResponseWithError<Error>>(cancellationToken).ConfigureAwait(false);

            // Expect a 204, which is NoContent
            response.HandleStatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  Check if the user is watching the specified label
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-content-contentId-get
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="label">string with the label</param>
        /// <param name="accountIdHolder">IAccountIdHolder for the user (account id), null for the current user</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>bool</returns>
        public static async Task<bool> IsLabelWatcher(this IUserDomain confluenceClient, string label, IAccountIdHolder accountIdHolder = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(label)) throw new ArgumentNullException(nameof(label));

            var userWatchLabelUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "watch", "label", label);

            // If there is no specified accountId, the current user is used
            if (accountIdHolder != null)
            {
                userWatchLabelUri = userWatchLabelUri.ExtendQuery("accountId", accountIdHolder.AccountId);
            }

            // Skip throwing on error, this is needed due to the fact that a unknown label returns a 403 (forbidden)
            var behavior = confluenceClient.Behaviour.ShallowClone();
            behavior.ThrowOnError = false;
            behavior.MakeCurrent();

            var response = await userWatchLabelUri.GetAsAsync<HttpResponse<UserWatch>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors(HttpStatusCode.OK, HttpStatusCode.Forbidden)?.IsWatching ?? false;
        }

        /// <summary>
        ///  Add the user to the list of users watching the specified space
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-space-spaceKey-get
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="spaceKey">string with the space key</param>
        /// <param name="accountIdHolder">IAccountIdHolder for the user (account id), null for the current user</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public static async Task AddSpaceWatcher(this IUserDomain confluenceClient, string spaceKey, IAccountIdHolder accountIdHolder = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(spaceKey)) throw new ArgumentNullException(nameof(spaceKey));

            var userWatchSpaceUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "watch", "space", spaceKey);

            // If there is no specified accountId, the current user is used
            if (accountIdHolder != null)
            {
                userWatchSpaceUri = userWatchSpaceUri.ExtendQuery("accountId", accountIdHolder.AccountId);
            }

            confluenceClient.Behaviour.MakeCurrent();
            var response = await userWatchSpaceUri.PostAsync<HttpResponseWithError<Error>>(null, cancellationToken: cancellationToken).ConfigureAwait(false);

            // Expect a 204, which is NoContent
            response.HandleStatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  Remove the user from the list of users watching the specified space
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-space-spaceKey-delete
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="spaceKey">string with the space key</param>
        /// <param name="accountIdHolder">IAccountIdHolder for the user (account id), null for the current user</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public static async Task DeleteSpaceWatcher(this IUserDomain confluenceClient, string spaceKey, IAccountIdHolder accountIdHolder = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(spaceKey)) throw new ArgumentNullException(nameof(spaceKey));

            var userWatchSpaceUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "watch", "space", spaceKey);

            // If there is no specified accountId, the current user is used
            if (accountIdHolder != null)
            {
                userWatchSpaceUri = userWatchSpaceUri.ExtendQuery("accountId", accountIdHolder.AccountId);
            }

            confluenceClient.Behaviour.MakeCurrent();
            var response = await userWatchSpaceUri.DeleteAsync<HttpResponseWithError<Error>>(cancellationToken).ConfigureAwait(false);

            // Expect a 204, which is NoContent
            response.HandleStatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        ///  Check if the user is watching the specified space
        ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-content-contentId-get
        /// </summary>
        /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
        /// <param name="spaceKey">string with the space key</param>
        /// <param name="accountIdHolder">IAccountIdHolder for the user (account id), null for the current user</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>bool</returns>
        public static async Task<bool> IsSpaceWatcher(this IUserDomain confluenceClient, string spaceKey, IAccountIdHolder accountIdHolder = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(spaceKey)) throw new ArgumentNullException(nameof(spaceKey));

            var userWatchLabelUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "watch", "space", spaceKey);

            // If there is no specified accountId, the current user is used
            if (accountIdHolder != null)
            {
                userWatchLabelUri = userWatchLabelUri.ExtendQuery("accountId", accountIdHolder.AccountId);
            }

            confluenceClient.Behaviour.MakeCurrent();
            var response = await userWatchLabelUri.GetAsAsync<HttpResponse<UserWatch>>(cancellationToken).ConfigureAwait(false);
            return response.HandleErrors().IsWatching;
        }
    }
}