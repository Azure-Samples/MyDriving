// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using NUnit.Framework;
using Xamarin.UITest;

namespace MyDriving.UITests
{
    [TestFixture(Platform.Android)]
    public class LoginTests : AbstractSetup
    {
        public LoginTests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void LoginWithFacebookTest()
        {
            ClearKeychain();

            new LoginPage()
                .LoginWithFacebook();
        }

        [Test]
        public void SkipAuthenticationTest()
        {
            ClearKeychain();

            new LoginPage()
                .SkipAuthentication();
        }
    }
}