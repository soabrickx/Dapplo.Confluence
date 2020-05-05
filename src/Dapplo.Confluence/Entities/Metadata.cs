// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Newtonsoft.Json;

namespace Dapplo.Confluence.Entities
{
    /// <summary>
    ///     Metadata information, used in attachment
    ///     See: https://docs.atlassian.com/confluence/REST/latest
    /// </summary>
    [JsonObject]
    public class Metadata
    {
        /// <summary>
        ///     A comment for the attachment
        /// </summary>
        [JsonProperty("comment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Comment { get; set; }

        /// <summary>
        ///     Type of media (content-type)
        /// </summary>
        [JsonProperty("mediaType", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string MediaType { get; set; }
        
        /// <summary>
        ///     Labels associated to content
        /// </summary>
        [JsonProperty("labels", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Result<Label> Labels { get; set; }
    }
}