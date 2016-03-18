// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using NUnit.Framework;
using Xamarin.UITest;

namespace MyDriving.UITests
{
    public class NavigationTests : AbstractSetup
    {
        public NavigationTests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void NavigateToCurrentTripTabTest()
        {
            new CurrentTripPage()
                .NavigateToCurrentTripPage();
        }

        [Test]
        public void NavigateToPastTripsTabTest()
        {
            new PastTripsPage()
                .NavigateToPastTripsPage();
        }

        [Test]
        public void NavigateToProfileTabTest()
        {
            new ProfilePage()
                .NavigateToProfilePage();
        }

        [Test]
        public void NavigateToSettingsTest()
        {
            new ProfilePage()
                .NavigateToProfilePage()
                .NavigateToSettingsPage();
        }
    }
}