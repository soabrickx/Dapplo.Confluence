// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Dapplo.Confluence.Entities
{
    /// <summary>
    ///     THis is the system information
    ///     See result in: https://developer.atlassian.com/cloud/confluence/rest/#api-api-settings-systemInfo-get
    /// </summary>
    [JsonObject]
    public class SystemInfoEntity
    {
        /// <summary>
        ///     The name which is displayed in the UI, usually "firstname lastname"
        /// </summary>
        [JsonProperty("cloudId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string CloudId { get; set; }
    }
}