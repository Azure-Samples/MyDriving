// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Xamarin.UITest;
using NUnit.Framework;

namespace MyDriving.UITests
{
	public class PastTripsTests : AbstractSetup
	{
		public PastTripsTests (Platform platform) : base (platform)
		{
		}

		[Test]
		public void PullToRefreshTest ()
		{
            new CurrentTripPage()
                .NavigateTo("Past Trips");

            new PastTripsPage()
				.PullToRefresh ();
		}

		[Test]
		public void NavigateToDetailsTest ()
		{
            new CurrentTripPage()
                .NavigateTo("Past Trips");

			new PastTripsPage ()
				.NavigateToPastTripsDetail ("James@ToVivace");

            new PastTripDetailPage()
                .AssertOnPage();
		}

		[Test]
		public void MoveTripSliderTest ()
		{
            new CurrentTripPage()
                .NavigateTo("Past Trips");

            new PastTripsPage()
                .NavigateToPastTripsDetail("James@ToVivace");

            new PastTripDetailPage()
				.MoveTripSlider ();
		}

		[Test]
		public void ClickTripSliderEndpointsTest ()
		{
            new CurrentTripPage()
                .NavigateTo("Past Trips");

            new PastTripsPage()
                .NavigateToPastTripsDetail("James@ToVivace");

            new PastTripDetailPage()
				.ClickTripSliderEndpoints ();
		}
	}
}