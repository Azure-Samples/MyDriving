using System;
using NUnit.Framework;
using Xamarin.UITest;

namespace MyDriving.UITests
{
	public class NavigationTests : AbstractSetup
	{
		public NavigationTests (Platform platform) : base (platform)
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
            new CurrentTripPage()
                .NavigateTo("Settings");

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
			new CurrentTripPage ()
				.NavigateToCurrentTripPage ();
		}
	}
}