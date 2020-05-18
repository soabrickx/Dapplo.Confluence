// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;

namespace Dapplo.Confluence.Entities
{
    /// <summary>
    /// Contains some basic paging information
    /// </summary>
    public class PagingInformation
    {
        /// <summary>
        ///     The result is limited by
        /// </summary>
        [JsonProperty("limit", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Limit { get; set; }

        /// <summary>
        ///     The start of the elements, this is used for paging
        /// </summary>
        [JsonProperty("start", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int? Start { get; set; }
    }
}
