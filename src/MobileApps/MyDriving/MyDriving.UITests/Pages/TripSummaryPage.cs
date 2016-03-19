using System;
using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
    public class TripSummaryPage : BasePage
    {
        public TripSummaryPage()
            : base("Trip Summary", "Trip Summary")
        {
        }
    }
}

