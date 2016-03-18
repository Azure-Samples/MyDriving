using System;
using Xamarin.UITest;
using NUnit.Framework;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
    public class PastTripsPage : BasePage
    {
        readonly Query RefreshView;

        public PastTripsPage()
            : base (x => x.Marked("Past Trips"), x => x.Class("UINavigationItemView").Marked("Past Trips"))
        {
            if (OniOS)
            {
                RefreshView = x => x.Class("UITableView");
            }
            if (OnAndroid)
            {
                RefreshView = x => x.Id("content_frame");
            }
		}

		public PastTripsPage PullToRefresh ()
		{
            app.WaitForElement(RefreshView);
            var coords = app.Query(RefreshView)[0].Rect;
            if (OniOS)
    			app.DragCoordinates(coords.CenterX, coords.Y + 75, coords.CenterX, coords.CenterY);
            if (OnAndroid)
                app.DragCoordinates(coords.CenterX, coords.Y, coords.CenterX, coords.CenterY);
            
            app.Screenshot("Pulled view to refresh");

			return this;
		}

        public void NavigateToPastTripsDetail (string title)
		{
            app.ScrollDownTo(title);
            app.Screenshot("Scrolled down to past trip: " + title);
            app.Tap(title);

			app.Screenshot ("Past Trips Detail");
		}
	}
}