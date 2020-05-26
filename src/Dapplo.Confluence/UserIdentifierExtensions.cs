// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.Confluence.Entities;

namespace Dapplo.Confluence
{
    /// <summary>
    ///     Extension for the IUserIdentifier
    /// </summary>
    public static class UserIdentifierExtensions
    {
        /// <summary>
        ///     Returns true if the supplied user identifier has a value
        /// </summary>
        /// <param name="userIdentifier">IUserIdentifier</param>
        /// <returns>bool</returns>
        public static bool HasIdentifier(this IUserIdentifier userIdentifier)
        {
            return !string.IsNullOrEmpty(userIdentifier?.AccountId) || !string.IsNullOrEmpty(userIdentifier?.Username);
        }
    }
}