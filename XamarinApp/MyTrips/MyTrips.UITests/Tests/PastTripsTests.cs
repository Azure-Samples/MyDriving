using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyTrips.UITests
{
	[TestFixture (Platform.iOS)]
	public class PastTripsTests : AbstractSetup
	{
		public PastTripsTests (Platform platform) : base (platform)
		{
		}

		[Test]
		public void PullToRefreshTest ()
		{
			new PastTripsPage ()
				.PullToRefresh ();
		}

		[Test]
		public void NavigateToDetailsTest ()
		{
			new PastTripsPage ()
				.NavigateToPastTripsDetail ();
		}

		[Test]
		public void MoveTripSliderTest ()
		{
			new PastTripsPage ()
				.NavigateToPastTripsDetail ()
				.MoveTripSlider ();
		}

		[Test]
		public void ClickTripSliderEndpointsTest ()
		{
			new PastTripsPage ()
				.NavigateToPastTripsDetail ()
				.ClickTripSliderEndpoints ();
		}
	}
}