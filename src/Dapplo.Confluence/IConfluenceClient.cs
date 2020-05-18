// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using Dapplo.Confluence.Entities;
using Dapplo.HttpExtensions;

namespace Dapplo.Confluence
{
	/// <summary>
	///     The is the interface to the base client functionality of the Confluence API
	/// </summary>
	public interface IConfluenceClient
	{
		/// <summary>
		///     The attachment domain
		/// </summary>
		IAttachmentDomain Attachment { get; }

		/// <summary>
		///     The base URI for your Confluence server api calls
		/// </summary>
		Uri ConfluenceApiUri { get; }

		/// <summary>
		///     The base URI for your Confluence server downloads
		/// </summary>
		Uri ConfluenceUri { get; }

		/// <summary>
		///     The content domain
		/// </summary>
		IContentDomain Content { get; }

        /// <summary>
        ///     The group domain
        /// </summary>
        IGroupDomain Group { get; }

        /// <summary>
		///     The misc domain
		/// </summary>
		IMiscDomain Misc { get; }

        /// <summary>
		///     Extensions of the client dock to this property, so typing "confluenceClient.Plugins." should show your extension.
		/// </summary>
		IConfluenceClientPlugins Plugins { get; }

		/// <summary>
		///     The space domain
		/// </summary>
		ISpaceDomain Space { get; }

		/// <summary>
		///     The user domain
		/// </summary>
		IUserDomain User { get; }

		/// <summary>
		///     Retrieve the Download as Uri from the supplied links object
		/// </summary>
		/// <param name="links">Links</param>
		/// <returns>Uri</returns>
		Uri CreateDownloadUri(Links links);

		/// <summary>
		///     Retrieve the TinyUi as Uri from the supplied links object
		/// </summary>
		/// <param name="links">Links</param>
		/// <returns>Uri</returns>
		Uri CreateTinyUiUri(Links links);

		/// <summary>
		///     Retrieve the WebUi as Uri from the supplied links object
		/// </summary>
		/// <param name="links">Links</param>
		/// <returns>Uri</returns>
		Uri CreateWebUiUri(Links links);

		/// <summary>
		///     Enables basic authentication for every request following this call
		/// </summary>
		/// <param name="user">string with the confluence user</param>
		/// <param name="password">string with the password for the confluence user</param>
		void SetBasicAuthentication(string user, string password);
	}

	/// <summary>
	///     Interface of all the confluence domain interfaces
	/// </summary>
	public interface IConfluenceDomain : IConfluenceClient
	{
		/// <summary>
		/// The IHttpBehaviour of the Confluence client
		/// </summary>
		IHttpBehaviour Behaviour { get; }
	}
}