// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Dapplo.Confluence.Entities;
using Dapplo.Confluence.Query;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.Confluence.Tests
{
    /// <summary>
    ///     Tests
    /// </summary>
    [CollectionDefinition("Dapplo.Confluence")]
    public class ContentTests
    {
        private static readonly LogSource Log = new LogSource();
        public ContentTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.ExceptionToStacktrace = exception => exception.ToStringDemystified();

            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
            _confluenceClient = ConfluenceClient.Create(TestConfluenceUri);

            var username = Environment.GetEnvironmentVariable("confluence_test_username");
            var password = Environment.GetEnvironmentVariable("confluence_test_password");
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                _confluenceClient.SetBasicAuthentication(username, password);
            }
        }

        // Test against a well known Confluence
        private static readonly Uri TestConfluenceUri = new Uri("https://greenshot.atlassian.net/wiki");

        private readonly IConfluenceClient _confluenceClient;

        [Fact]
        public async Task Test_ContentVersion()
        {
            var query = Where.And(Where.Space.Is("TEST"), Where.Type.IsPage, Where.Title.Contains("Test Home"));
            var searchResults = await _confluenceClient.Content.SearchAsync(query);
            var searchResult = searchResults.First();
            Log.Info().WriteLine("Version = {0}", searchResult.Version.Number);
            query = Where.Title.Contains("Test Home");
            searchResults = await _confluenceClient.Content.SearchAsync(query);
            searchResult = searchResults.First();
            Log.Info().WriteLine("Version = {0}", searchResult.Version.Number);

            var content = await _confluenceClient.Content.GetAsync(searchResult, ConfluenceClientConfig.ExpandGetContentWithStorage);
            Log.Info().WriteLine("Version = {0}", content.Version.Number);
        }

        /// <summary>
        ///     Test GetAsync
        /// </summary>
        //[Fact]
        public async Task TestGetContent()
        {
            var content = await _confluenceClient.Content.GetAsync(950274);
            Assert.NotNull(content);
            Assert.NotNull(content.Version);
            Assert.NotNull(content.Ancestors);
            Assert.True(content.Ancestors.Count > 0);
        }

        /// <summary>
        ///     Test .GetChildren
        /// </summary>
        [Fact]
        public async Task TestGetChildren()
        {
            var results = await _confluenceClient.Content.GetChildrenAsync(550731777, limit: 1);
            Assert.NotNull(results);
            Assert.True(results.HasNext);
            Assert.True(results.Size > 0);
        }

        /// <summary>
        ///     Test GetHistoryAsync
        /// </summary>
        //[Fact]
        public async Task TestGetContentHistory()
        {
            var history = await _confluenceClient.Content.GetHistoryAsync(950274);
            Assert.NotNull(history);
            Assert.NotNull(history.CreatedBy);
        }

        [Fact]
        public async Task TestCreateContent()
        {
            var query = Where.And(Where.Space.Is("TEST"), Where.Type.IsPage, Where.Title.Contains("Testing 1 2 3"));
            var searchResults = await _confluenceClient.Content.SearchAsync(query);
            var oldPage = searchResults.Results.FirstOrDefault();
            if (oldPage != null)
            {
                await _confluenceClient.Content.DeleteAsync(oldPage);
            }
            await Task.Delay(1000);
            var page = await _confluenceClient.Content.CreateAsync(ContentTypes.Page, "Testing 1 2 3", "TEST", "<p>This is a test</p>");
            Assert.NotNull(page);
            Assert.True(page.Id > 0);
            await Task.Delay(1000);
            await _confluenceClient.Content.DeleteAsync(page);
        }

        //[Fact]
        public async Task TestDeleteContent()
        {
            await _confluenceClient.Content.DeleteAsync(30375945);
        }

        [Fact]
        public async Task TestSearch()
        {
            ConfluenceClientConfig.ExpandSearch = new[] { "version", "space", "space.icon", "space.description", "space.homepage", "history.lastUpdated" };

            var searchResult = await _confluenceClient.Content.SearchAsync(Where.And(Where.Type.IsPage, Where.Text.Contains("Test Home")), limit: 1);
            Assert.Equal(ContentTypes.Page, searchResult.First().Type);
            var uri = _confluenceClient.CreateWebUiUri(searchResult.FirstOrDefault()?.Links);
            Assert.NotNull(uri);
        }

        [Fact]
        public async Task TestSearchAttachment()
        {
            ConfluenceClientConfig.ExpandSearch = new[] { "version", "space", "space.icon", "space.description", "space.homepage", "history.lastUpdated" };

            var query = Where.And(Where.Type.IsAttachment, Where.Text.Contains("404"));

            var searchResult = await _confluenceClient.Content.SearchAsync(query, limit: 1);
            var attachment = searchResult.First();
            Assert.Equal(ContentTypes.Attachment, attachment.Type);
            Assert.NotNull(_confluenceClient.Attachment.CreateDownloadUri(attachment.Links));
            // I know the attachment is a bitmap, this should work
            var bitmap = await _confluenceClient.Attachment.GetContentAsync<Bitmap>(attachment);
            Assert.True(bitmap.Width > 0);
        }
        
        [Fact]
        public async Task TestSearchLabels()
        {
            ConfluenceClientConfig.ExpandSearch = new[] { "version", "space", "space.icon", "space.description", "space.homepage", "history.lastUpdated", "metadata.labels" };

            var searchResult = await _confluenceClient.Content.SearchAsync(Where.And(Where.Type.IsPage, Where.Text.Contains("Test Home")), limit: 1);
            Assert.NotEmpty(searchResult.First().Metadata.Labels.Results);
        }

        //[Fact]
        public async Task TestLabels()
        {
            var searchResult = await _confluenceClient.Content.SearchAsync(Where.And(Where.Type.IsPage, Where.Text.Contains("Test Home")), limit: 1);
            var contentId = searchResult.First().Id;

            var labels = new[] { "test1", "test2" };
            await _confluenceClient.Content.AddLabelsAsync(contentId, labels.Select(s => new Label { Name = s }));
            var labelsForContent = await _confluenceClient.Content.GetLabelsAsync(contentId);
            Assert.Equal(labels.Length, labelsForContent.Count(label => labels.Contains(label.Name)));

            // Delete all
            foreach (var label in labelsForContent)
            {
                await _confluenceClient.Content.DeleteLabelAsync(contentId, label.Name);
            }
        }
    }
}