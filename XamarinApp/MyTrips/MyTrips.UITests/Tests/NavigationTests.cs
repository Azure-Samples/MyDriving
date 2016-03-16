using System;
using NUnit.Framework;
using Xamarin.UITest;

namespace MyTrips.UITests
{
	public class NavigationTests : AbstractSetup
	{
		public NavigationTests (Platform platform) : base (platform)
		{
		}

		[Test]
		public void NavigateToProfileTabTest ()
		{
			new ProfilePage ()
				.NavigateToProfilePage ();
		}

		[Test]
		public void NavigateToSettingsTest ()
		{
			new ProfilePage ()
				.NavigateToProfilePage ()
				.NavigateToSettingsPage ();
		}

		[Test]
		public void NavigateToPastTripsTabTest ()
		{
			new PastTripsPage ()
				.NavigateToPastTripsPage ();
		}

		[Test]
		public void NavigateToCurrentTripTabTest ()
		{
			new CurrentTripPage ()
				.NavigateToCurrentTripPage ();
		}
	}
}