// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.Confluence
{
	/// <summary>
	///     Use this class to configure some of the behaviour.
	///     This stores the "expand" settings for the different REST calls, and defines what additional information is
	///     requested.
	/// </summary>
	public static class ConfluenceClientConfig
	{
		/// <summary>
		///     These expand values can be used when getting the content with storage, instead of view
		/// </summary>
		public static string[] ExpandGetContentWithStorage { get; set; } = { "body", "body.storage", "version" };

		/// <summary>
		///     The values that are expanded in the GetAttachments result
		/// </summary>
		public static string[] ExpandGetAttachments { get; set; } = { "version", "container" };

		/// <summary>
		///     The values that are expanded in the GetChildren results
		/// </summary>
		public static string[] ExpandGetChildren { get; set; } = { "page" };

		/// <summary>
		///     The values that are expanded in the GetContent result
		/// </summary>
		public static string[] ExpandGetContent { get; set; } = { "body", "body.view", "version" };

		/// <summary>
		///     The values that are expanded in the GetContent result, but for an update use case
		/// </summary>
		public static string[] ExpandGetContentForUpdate { get; set; } = { "body.storage", "version" };

		/// <summary>
		///     The values that are expanded in the GetContentByTitle results
		/// </summary>
		public static string[] ExpandGetContentByTitle { get; set; }

		/// <summary>
		///     The values that are expanded in the Space.GetContentsAsync results
		/// </summary>
		public static string[] ExpandSpaceGetContents { get; set; }

		/// <summary>
		///     The values that are expanded in the GetSpace result
		/// </summary>
		public static string[] ExpandGetSpace { get; set; } = { "icon", "description.plain", "homepage" };

		/// <summary>
		///     The values that are expanded in the GetSpaces results
		/// </summary>
		public static string[] ExpandGetSpaces { get; set; } = { "icon", "description.plain", "homepage" };

		/// <summary>
		///     The values that are expanded in the Search results
		/// </summary>
		public static string[] ExpandSearch { get; set; } = { "version", "space", "space.icon", "space.description", "space.homepage" };
	}
}