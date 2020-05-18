// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Dapplo.Confluence.Entities
{
    /// <summary>
    ///     UserWatch
    ///     See result in: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-label-labelName-get
    /// </summary>
    [JsonObject]
    public class UserWatch
    {
        /// <summary>
        ///     The name which is displayed in the UI, usually "firstname lastname"
        /// </summary>
        [JsonProperty("watching", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsWatching { get; set; }
    }
}