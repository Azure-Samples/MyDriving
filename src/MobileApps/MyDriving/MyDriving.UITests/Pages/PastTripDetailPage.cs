using System;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
    public class PastTripDetailPage : BasePage
    {
        readonly Query TripProgressSlider;

        public PastTripDetailPage()
            : base("stats", "ios")
        {
            if (OniOS)
            { 
                TripProgressSlider = x => x.Class("UISlider");
            }
            if (OnAndroid)
            {
                TripProgressSlider = x => x.Id("trip_progress");
            }
        }

        public PastTripDetailPage MoveTripSlider()
        {
            if (OniOS)
            {
                app.SetSliderValue(TripProgressSlider, 25);
                app.Screenshot("Trip Slider at 25%");

                app.SetSliderValue(TripProgressSlider, 75);
                app.Screenshot("Trip Slider at 75%");
            }
            if (OnAndroid)
            {
                app.SetSliderValue(TripProgressSlider, 250);
                app.Screenshot("Trip Slider at 25%");

                app.SetSliderValue(TripProgressSlider, 750);
                app.Screenshot("Trip Slider at 75%");
            }
            return this;
        }

        public PastTripDetailPage ClickTripSliderEndpoints()
        {
            if (OniOS)
            {
                app.Tap(x => x.Text("A"));
                app.Screenshot("Tapped A Endpoint");

                app.Tap(x => x.Text("B"));
                app.Screenshot("Tapped B Endpoint");
            }
            //TODO: See if this can do anything on android

            return this;
        }
    }
}

