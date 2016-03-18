// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace MyDriving.UITests
{
    public class CurrentTripPage : BasePage
    {
        public CurrentTripPage()
        {
            App.Screenshot("Current Trip");
        }

        public CurrentTripPage NavigateToCurrentTripPage()
        {
            App.Tap("Current Trip");

            return this;
        }

        public CurrentTripPage StartRecordingTrip()
        {
            App.Tap(c => c.Class("UIButton"));
            App.Screenshot("Started recording trip");

            App.Tap("Use Simulator");

            return this;
        }

        public CurrentTripPage StopRecordingTrip()
        {
            System.Threading.Thread.Sleep(2500);
            App.Tap(c => c.Class("UIButton"));

            return this;
        }

        public CurrentTripPage EnterTripName()
        {
            App.EnterText(c => c.Class("UITextField"), "Trip Name");
            App.DismissKeyboard();
            App.Screenshot("Trip summary");

            return this;
        }

        public CurrentTripPage DismissTripSummary()
        {
            App.Tap("Done");

            return this;
        }
    }
}