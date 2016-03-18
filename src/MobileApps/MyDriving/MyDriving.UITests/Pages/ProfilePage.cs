// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace MyDriving.UITests
{
    public class ProfilePage : BasePage
    {
        public ProfilePage()
        {
            App.Screenshot("Profile Page");
        }

        public ProfilePage NavigateToProfilePage()
        {
            App.Tap("Profile");

            return this;
        }

        public ProfilePage NavigateToSettingsPage()
        {
            NavigateTo("Settings");
            return this;
        }

        public ProfilePage NavigateFromSettingsDetailPage()
        {
            App.Tap("Settings");

            return this;
        }

        public ProfilePage SetDistanceSetting()
        {
            App.Tap("Metric (km)");
            App.Screenshot("Set Distance Setting");

            return this;
        }

        public ProfilePage SetCapacitySetting()
        {
            App.Tap("Metric (liters)");
            App.Screenshot("Set Capacity Setting");

            return this;
        }

        public ProfilePage NavigateToDistanceSetting()
        {
            App.Tap("Distance");

            return this;
        }

        public ProfilePage NavigateToCapacitySetting()
        {
            App.Tap("Capacity");

            return this;
        }

        public ProfilePage NavigateToDeviceConnectionSetting()
        {
            App.Tap("Device connection string");

            return this;
        }

        public ProfilePage SetDeviceConnectionSetting()
        {
            App.EnterText(x => x.Class("UITextField"), "http://build2016.azurewebsites.net");
            App.Screenshot("Set Device Connection String Setting");

            return this;
        }

        public ProfilePage NavigateToMobileClientUrlSetting()
        {
            App.Tap("Mobile client URL");

            return this;
        }

        public ProfilePage SetMobileClientUrlSetting()
        {
            App.EnterText(x => x.Class("UITextField"), "http://build2016.azurewebsites.net");
            App.Screenshot("Set Mobile Client Url Setting");

            return this;
        }

        public ProfilePage Logout()
        {
            App.ScrollDownTo("Log Out");
            App.Tap(x => x.Marked("Log Out"));
            App.Tap(x => x.Marked("Yes, Logout"));

            return this;
        }
    }
}