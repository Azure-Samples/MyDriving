// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyDriving.UITests
{
    public class SettingsTests : AbstractSetup
    {
        public SettingsTests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void ChangeCapacityUnits()
        {
            new ProfilePage()
                .NavigateToProfilePage()
                .NavigateToSettingsPage()
                .NavigateToCapacitySetting()
                .SetCapacitySetting()
                .NavigateFromSettingsDetailPage();

            app.Screenshot("Changed Capacity Setting");
        }

        [Test]
        public void ChangeDistanceUnits()
        {
            new ProfilePage()
                .NavigateToProfilePage()
                .NavigateToSettingsPage()
                .NavigateToDistanceSetting()
                .SetDistanceSetting()
                .NavigateFromSettingsDetailPage();

            app.Screenshot("Changed Distance Setting");
        }


        [Test]
        public void Logout()
        {
            new ProfilePage()
                .NavigateToProfilePage()
                .NavigateToSettingsPage()
                .Logout();

            app.Screenshot("Logged out");
        }
    }
}