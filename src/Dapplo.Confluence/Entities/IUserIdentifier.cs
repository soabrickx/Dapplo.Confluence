// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.Confluence.Entities
{
    /// <summary>
    /// Interface which allows us to have an account ID or username to work with.
    /// </summary>
    public interface IUserIdentifier
    {
        /// <summary>
        /// For Confluence Cloud this contains the account ID of the user, which uniquely identifies the user across all Atlassian products.
        /// </summary>
        string AccountId { get; }

        /// <summary>
        /// Provide the username for the Confluence Server (non cloud)
        /// </summary>
        string Username { get; }
    }
}
