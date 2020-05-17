// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.Confluence.Tests
{
    /// <summary>
    ///     Tests for the user domain
    /// </summary>
    [CollectionDefinition("Dapplo.Confluence")]
    public class UserDomainTests : ConfluenceIntegrationTests
    {
        public UserDomainTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        /// <summary>
        ///     Test if the current user is correctly retrieved
        /// </summary>
        [Fact]
        public async Task TestCurrentUser()
        {
            var currentUser = await ConfluenceTestClient.User.GetCurrentUserAsync();
            Assert.NotNull(currentUser);
            Assert.NotNull(currentUser.AccountId);
            Assert.NotNull(currentUser.ProfilePicture);
            Assert.DoesNotContain("Anonymous", currentUser.DisplayName);
        }

        /// <summary>
        ///     Test if the picture can be downloaded
        /// </summary>
        [Fact]
        public async Task TestCurrentUserPicture()
        {
            var currentUser = await ConfluenceTestClient.User.GetCurrentUserAsync();
            Assert.NotNull(currentUser);
            Assert.NotNull(currentUser.ProfilePicture);
            Assert.DoesNotContain("Anonymous", currentUser.DisplayName);

            var bitmapSource = await ConfluenceTestClient.Misc.GetPictureAsync<BitmapSource>(currentUser.ProfilePicture);
            Assert.NotNull(bitmapSource);
            Assert.True(bitmapSource.Width > 0);
        }

        /// <summary>
        /// Test if the GetGroupMembershipsAsync returns at least a group
        /// </summary>
        [Fact]
        public async Task TestGetGroupMembershipsAsync()
        {
            var currentUser = await ConfluenceTestClient.User.GetCurrentUserAsync();
            var groupsForUser = await ConfluenceTestClient.User.GetGroupMembershipsAsync(currentUser);
            Assert.NotEmpty(groupsForUser);
        }
    }
}