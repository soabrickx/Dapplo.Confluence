// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dapplo.Confluence.Entities
{
    /// <summary>
    ///     A container to store pageable results
    ///     See: https://docs.atlassian.com/confluence/REST/latest
    /// </summary>
    [JsonObject]
    public class Result<TResult> : PagingInformation, IEnumerable<TResult>
    {
        /// <summary>
        ///     Different links for this entity, depending on the entry
        /// </summary>
        [JsonProperty("_links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Links Links { get; set; }

        /// <summary>
        ///     The actual requested information
        /// </summary>
        [JsonProperty("results", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<TResult> Results { get; set; }

        /// <summary>
        ///     How many elements
        /// </summary>
        [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Size { get; set; }

        /// <summary>
        /// Returns if there are more results as requested
        /// </summary>
        [JsonIgnore]
        public bool HasNext => Links?.Next != null;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<TResult> GetEnumerator()
        {
            return Results.GetEnumerator();
        }
    }
}