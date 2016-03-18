using System;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
    public class PastTripDetailPage : BasePage
    {
        readonly Query TripProgressSlider;

        public PastTripDetailPage()
            : base(x => x.Marked("stats"), x => x.Class("UISlider"))
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
            app.SetSliderValue(TripProgressSlider, 250);
            app.Screenshot("Trip Slider at 25%");

            app.SetSliderValue(TripProgressSlider, 750);
            app.Screenshot("Trip Slider at 75%");

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

