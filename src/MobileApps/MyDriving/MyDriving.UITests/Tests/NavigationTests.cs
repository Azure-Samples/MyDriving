// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

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
		public void NavigateToProfileTabTest ()
		{
            new CurrentTripPage()
                .NavigateTo("Profile");

            new ProfilePage()
                .AssertOnPage();
		}

		[Test]
		public void NavigateToSettingsTest ()
		{
            if (OnAndroid)
            { 
                new CurrentTripPage()
                    .NavigateTo("Settings");
            }

            if (OniOS)
            {
                new CurrentTripPage()
                    .NavigateTo("Profile");

                new ProfilePage()
                    .NavigateToSettings();
            }

            new SettingsPage()
                .AssertOnPage();
		}

		[Test]
		public void NavigateToPastTripsTabTest ()
		{
            new CurrentTripPage()
                .NavigateTo("Past Trips");

            new PastTripsPage()
                .AssertOnPage();
		}

		[Test]
		public void NavigateToCurrentTripTabTest ()
		{
            new CurrentTripPage()
                .NavigateTo("Current Trip");

            new CurrentTripPage()
                .AssertOnPage();
		}
	}
}