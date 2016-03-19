﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace MyDriving.UITests
{
    public class PastTripsPage : BasePage
    {
        string pageId;
        string sliderId;

        public PastTripsPage()
        {
            if (OniOS)
            {
                sliderId = "UISlider";
                pageId = "Trips";
            }
            else
            {
                sliderId = "SeekBar";
                pageId = "Past Trips";
            }
        }

        public PastTripsPage NavigateToPastTripsPage()
        {
            NavigateTo(pageId);

            return this;
        }

        public PastTripsPage PullToRefresh()
        {
            var coords = App.Query("TableView")[0].Rect;
            App.DragCoordinates(coords.CenterX, 0, coords.CenterX, coords.Y);

            return this;
        }

        public PastTripsPage NavigateToPastTripsDetail()
        {
            App.Tap(x => x.Marked("James@ToVivace"));
            App.Screenshot("Past Trips Detail");

            return this;
        }

        public PastTripsPage MoveTripSlider()
        {
            App.SetSliderValue(c => c.Class(sliderId), 25);
            App.Screenshot("Trip Slider at 25%");

            App.SetSliderValue(c => c.Class(sliderId), 75);
            App.Screenshot("Trip Slider at 75%");

            return this;
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