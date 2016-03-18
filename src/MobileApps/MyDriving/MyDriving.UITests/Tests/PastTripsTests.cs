using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyDriving.UITests
{
    [TestFixture(Platform.Android)]
	public class PastTripsTests : AbstractSetup
	{
		public PastTripsTests (Platform platform) : base (platform)
		{
		}

		[Test]
		public void PullToRefreshTest ()
		{
            new PastTripsPage()
                .NavigateToPastTripsPage()
				.PullToRefresh ();
		}

		[Test]
		public void NavigateToDetailsTest ()
		{
			new PastTripsPage ()
                .NavigateToPastTripsPage()
				.NavigateToPastTripsDetail ();
		}

		[Test]
		public void MoveTripSliderTest ()
		{
			new PastTripsPage ()
                .NavigateToPastTripsPage()
				.NavigateToPastTripsDetail ()
				.MoveTripSlider ();
		}

		[Test]
		public void ClickTripSliderEndpointsTest ()
		{
			new PastTripsPage ()
                .NavigateToPastTripsPage()
				.NavigateToPastTripsDetail ()
				.ClickTripSliderEndpoints ();
		}
	}
}