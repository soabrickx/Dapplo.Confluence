// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.Confluence.Tests
{
    /// <summary>
    ///     Tests
    /// </summary>
    [CollectionDefinition("Dapplo.Confluence")]
    public class ConfluenceTests : ConfluenceIntegrationTests
    {
        public ConfluenceTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        /// <summary>
        ///     Test only works on Confluence 6.6 and later
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestCurrentUserAndPicture()
        {
            var currentUser = await ConfluenceTestClient.User.GetCurrentUserAsync();
            Assert.NotNull(currentUser);
            Assert.NotNull(currentUser.ProfilePicture);
            Assert.DoesNotContain("Anonymous", currentUser.DisplayName);

            var bitmapSource = await ConfluenceTestClient.Misc.GetPictureAsync<MemoryStream>(currentUser.ProfilePicture);
            Assert.NotNull(bitmapSource);
        }
    }
}