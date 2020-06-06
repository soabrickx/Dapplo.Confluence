// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit.Abstractions;

namespace Dapplo.Confluence.Tests
{
    /// <summary>
    ///     Base class for integration tests
    /// </summary>
    public abstract class ConfluenceIntegrationTests
    {
        // Test against a "well known" Confluence
        private static readonly Uri TestConfluenceUri = new Uri("http://n40l.fritz.box:8090");

        protected readonly IConfluenceClient ConfluenceTestClient;

        protected ConfluenceIntegrationTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.ExceptionToStacktrace = exception => exception.ToStringDemystified();

            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
            ConfluenceTestClient = ConfluenceClient.Create(TestConfluenceUri);

            var username = "dapplounittests";
            var password = "DapploUnitTests";
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                ConfluenceTestClient.SetBasicAuthentication(username, password);
            }
        }
    }
}