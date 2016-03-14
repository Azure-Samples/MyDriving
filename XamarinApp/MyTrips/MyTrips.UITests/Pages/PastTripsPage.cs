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

		public PastTripsPage NavigateToPastTripsPage ()
		{
			app.Tap ("Trips");

			return this;
		}

		public PastTripsPage PullToRefresh ()
		{
			var coords = app.Query("TableView")[0].Rect;
			app.DragCoordinates(coords.CenterX, 0, coords.CenterX, coords.Y);

			return this;
		}

		public PastTripsPage NavigateToPastTripsDetail ()
		{
			app.Tap(x => x.Marked("James@ToVivace"));
			app.Screenshot ("Past Trips Detail");

			return this;
		}

		public PastTripsPage MoveTripSlider ()
		{
			app.SetSliderValue (c => c.Class ("UISlider"), 25);
			app.Screenshot ("Trip Slider at 25%");

			app.SetSliderValue (c => c.Class ("UISlider"), 75);
			app.Screenshot ("Trip Slider at 75%");

			return this;
		}

		public PastTripsPage ClickTripSliderEndpoints ()
		{
			app.Tap(x => x.Text("A"));
			app.Screenshot ("Tapped A Endpoint");

			app.Tap(x => x.Text("B"));
			app.Screenshot ("Tapped B Endpoint");

			return this;
		}
	}
}