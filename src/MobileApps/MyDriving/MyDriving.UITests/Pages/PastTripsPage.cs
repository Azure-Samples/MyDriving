using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyDriving.UITests
{
    public class PastTripsPage : BasePage
    {
        string SliderId;
        string PageId;
        public PastTripsPage()
            : base ("android_trait", "ios_trait")
        {
            if (OniOS)
            {
                SliderId = "UISlider";
                PageId = "Trips";
            }
            else if (OnAndroid)
            {
                SliderId = "SeekBar";
                PageId = "Past Trips";
            }

			
		}

		public PastTripsPage NavigateToPastTripsPage ()
		{
            NavigateTo(PageId);

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
			app.SetSliderValue (c => c.Class (SliderId), 25);
			app.Screenshot ("Trip Slider at 25%");

			app.SetSliderValue (c => c.Class (SliderId), 75);
			app.Screenshot ("Trip Slider at 75%");

			return this;
		}

		public PastTripsPage ClickTripSliderEndpoints ()
		{
            if (OniOS)
            {
                app.Tap(x => x.Text("A"));
                app.Screenshot("Tapped A Endpoint");

                app.Tap(x => x.Text("B"));
                app.Screenshot ("Tapped B Endpoint");
            }

			return this;
		}
	}
}