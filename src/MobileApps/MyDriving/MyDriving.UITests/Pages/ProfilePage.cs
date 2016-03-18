// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

﻿using System;
using NUnit.Framework;
using Xamarin.UITest;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
	public class ProfilePage : BasePage
	{
        readonly Query FuelConsumptionField;
        readonly Query DistanceField;
        readonly Query SettingsTab;

		public ProfilePage ()
            : base (x => x.Marked("profile_image"), x => x.Raw("* {text CONTAINS 'Driving Skills: '}"))
		{
            if (OnAndroid)
            { 
                FuelConsumptionField = x => x.Id("text_fuel_consumption");
                DistanceField = x => x.Id("text_distance");
            }
            if (OniOS)
            {
                SettingsTab = x => x.Id("tab_Settings.png");
                FuelConsumptionField = x => x.Class("ProfileStatCell").Descendant().Marked("Fuel Consumption").Sibling();
                DistanceField = x => x.Class("ProfileStatCell").Descendant().Marked("Total Distance").Sibling();
            }
		}

        public ProfilePage CheckFuelMetric(bool metric)
        {
            App.ScrollDownTo(FuelConsumptionField);
            var fuelText = App.Query(FuelConsumptionField)[0].Text;
            App.Screenshot("Verifying fuel units correct");

            var expectedUnits = metric ? "l" : "gal";
            StringAssert.Contains(expectedUnits, fuelText, message:"Couldnt verify fuel units");

            return this;
        }

        public ProfilePage CheckDistanceMetric(bool metric)
        {
            App.ScrollDownTo(DistanceField);
            var distanceText = App.Query(DistanceField)[0].Text;
            App.Screenshot("Verifying distance units correct");

            var expectedUnits = metric ? "km" : "miles";
            StringAssert.Contains(expectedUnits, distanceText, message: "Couldnt verify distance units");

            return this;
        }

        public void NavigateToSettings()
        {
            if (OnAndroid)
                return;

            App.Tap(SettingsTab);
            App.Screenshot("Tapped Settings Tab");
        }
	}
}