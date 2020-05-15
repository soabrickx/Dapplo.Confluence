// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using Newtonsoft.Json;

namespace Dapplo.Confluence.Entities
{
    /// <summary>
    ///     User information
    ///     See: https://docs.atlassian.com/confluence/REST/latest
    /// </summary>
    [JsonObject]
    public class User : IAccountIdHolder
    {
        /// <summary>
        ///     The name which is displayed in the UI, usually "firstname lastname"
        /// </summary>
        [JsonProperty("displayName", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DisplayName { get; set; }

        /// <summary>
        ///     The public name or nickname of the user.Will always contain a value.
        /// </summary>
        [JsonProperty("publicName", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string PublicName { get; set; }

        /// <summary>
        ///     Information on the profile picture
        /// </summary>
        [JsonProperty("profilePicture", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Picture ProfilePicture { get; set; }

        /// <summary>
        ///     The email address of the user. Depending on the user's privacy setting, this may return an empty string.
        /// </summary>
        [JsonProperty("email", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Email { get; set; }

        /// <summary>
        ///     Valid values: known, unknown, anonymous, user
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type { get; set; }

        /// <summary>
        ///     The account type of the user, may return empty string if unavailable.
        ///     Valid values: atlassian, app (if this user is a bot user created on behalf of an Atlassian app)
        /// </summary>
        [JsonProperty("accountType", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string AccountType { get; set; }

        /// <summary>
        ///     This property is no longer available and will be removed from the documentation soon. Use accountId instead.
        /// </summary>
        [JsonProperty("userKey", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [Obsolete("UserKey is deprecated, see: https://developer.atlassian.com/cloud/confluence/deprecation-notice-user-privacy-api-migration-guide/")]
        public string UserKey { get; set; }

        /// <summary>
        ///     This property is no longer available and will be removed from the documentation soon. Use accountId instead.
        /// </summary>
        [JsonProperty("username", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [Obsolete("Username is deprecated, see: https://developer.atlassian.com/cloud/confluence/deprecation-notice-user-privacy-api-migration-guide/")]
        public string Username { get; set; }

        /// <summary>
        ///     The personal space for the user
        /// </summary>
        [JsonProperty("personalSpace", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Space PersonalSpace { get; set; }

        /// <inheritdoc cref="IAccountIdHolder" />
        [JsonProperty("accountId")]
        public string AccountId { get; set; }
    }
}