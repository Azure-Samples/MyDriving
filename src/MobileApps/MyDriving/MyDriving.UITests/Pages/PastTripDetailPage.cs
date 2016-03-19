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
            App.SetSliderValue(TripProgressSlider, 250);
            App.Screenshot("Trip Slider at 25%");

            App.SetSliderValue(TripProgressSlider, 750);
            App.Screenshot("Trip Slider at 75%");

            return this;
        }

        public PastTripDetailPage ClickTripSliderEndpoints()
        {
            if (OniOS)
            {
                App.Tap(x => x.Text("A"));
                App.Screenshot("Tapped A Endpoint");

                App.Tap(x => x.Text("B"));
                App.Screenshot("Tapped B Endpoint");
            }

            return this;
        }
    }
}

