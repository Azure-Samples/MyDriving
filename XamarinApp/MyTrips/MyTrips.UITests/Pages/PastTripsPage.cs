using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyTrips.UITests
{
	public class PastTripsPage : BasePage
	{
		public PastTripsPage ()
		{
			app.Screenshot ("Past Trips Page");
		}

		public void NavigateToPastTripsPage ()
		{
			app.Tap ("Trips");
		}

		public void PullToRefresh ()
		{

		}

		public void NavigateToPastTripsDetail ()
		{

		}

		public void MoveTripSlider ()
		{

		}

		public void ClickTripSliderEndpoints ()
		{

		}
	}
}