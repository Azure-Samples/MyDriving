// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

﻿using System;
using System.Linq;
using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
    public class CurrentTripPage : BasePage
    {
        readonly Query RecordingButton;
        readonly string UseSimulatorButton = "Use Simulator";
        readonly Query TripTitleField;
        readonly Query SaveTripButton;

        public CurrentTripPage ()
            : base("Current Trip", "Current Trip")
        {
            if (OnAndroid)
            {
                RecordingButton = x => x.Id("fab");
                TripTitleField = x => x.Class("EditText");
                SaveTripButton = x => x.Id("button1");
            }
            if (OniOS)
            { 
                RecordingButton = x => x.Class("UIButton");
                SaveTripButton = x => x.Marked("OK");
                TripTitleField = x => x.Class("UITextField");
            }
        }

        public CurrentTripPage StartRecordingTrip ()
        {
            App.Tap (RecordingButton);

            System.Threading.Thread.Sleep(2500);

            if (!App.Query(UseSimulatorButton).Any())
            {
                App.Tap(RecordingButton);
            }

            App.Tap(UseSimulatorButton);

            App.Screenshot ("Started recording trip");

            return this;
        }

        public CurrentTripPage StopRecordingTrip ()
        {
            System.Threading.Thread.Sleep (2500);
            App.Tap (RecordingButton);
            App.Screenshot("Stopped recording trip");

            return this;
        }

        public CurrentTripPage SaveTrip (string title)
        {
            App.ClearText (TripTitleField);
            App.EnterText (TripTitleField, title);
            App.DismissKeyboard ();

            App.Screenshot ("Trip title entered");

            App.Tap(SaveTripButton);
            App.Screenshot("Trip Saved!");

            return this;
        }
    }
}