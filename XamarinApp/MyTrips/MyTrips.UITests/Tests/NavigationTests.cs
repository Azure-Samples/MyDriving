using System;
using NUnit.Framework;
using Xamarin.UITest;

namespace MyTrips.UITests
{
	//[TestFixture (Platform.iOS)]
	public class NavigationTests : AbstractSetup
	{
		public NavigationTests (Platform platform) : base (platform)
		{
		}

		[Test]
		public void NavigateToProfileTab ()
		{
			new ProfilePage ()
				.NavigateToProfilePage ();
		}

		[Test]
		public void NavigateToSettings ()
		{
			new ProfilePage ()
				.NavigateToProfilePage ()
				.NavigateToSettingsPage ();
		}

		[Test]
		public void NavigateToPastTripsTab ()
		{
			new PastTripsPage ()
				.NavigateToPastTripsPage ();
		}

		[Test]
		public void NavigateToCurrentTripTab ()
		{
			new CurrentTripPage ()
				.NavigateToCurrentTripPage ();
		}
	}
}