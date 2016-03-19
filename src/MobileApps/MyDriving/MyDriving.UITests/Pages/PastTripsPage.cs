// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
    public class PastTripsPage : BasePage
    {
        readonly Query RefreshView;
        readonly Func<string, Query> PastTripCell;

        public PastTripsPage()
            : base (x => x.Marked("Past Trips"), x => x.Class("UINavigationItemView").Marked("Past Trips"))
        {
            if (OniOS)
            {
                RefreshView = x => x.Class("UITableView");
                PastTripCell = (arg) => x => x.Marked(arg).Parent().Class("TripTableViewCellWithImage");
            }
            if (OnAndroid)
            {
                RefreshView = x => x.Id("content_frame");
            }
		}

		public PastTripsPage PullToRefresh ()
		{
            App.WaitForElement(RefreshView);
            var coords = App.Query(RefreshView)[0].Rect;
            if (OniOS)
    			App.DragCoordinates(coords.CenterX, coords.Y + 75, coords.CenterX, coords.CenterY);
            if (OnAndroid)
                App.DragCoordinates(coords.CenterX, coords.Y, coords.CenterX, coords.CenterY);
            
            App.Screenshot("Pulled view to refresh");

            return this;
        }

        public void NavigateToPastTripsDetail (string title)
		{
            if (OnAndroid)
            {
                App.ScrollDownTo(title);
                App.Screenshot("Selecting past trip: " + title);
                App.Tap(title);
            }
            if (OniOS)
            {
                App.ScrollDownTo(PastTripCell(title));
                App.Screenshot("Selecting past trip: " + title);
                App.Tap(PastTripCell(title));
            }
		}

        public PastTripsPage ClickTripSliderEndpoints()
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