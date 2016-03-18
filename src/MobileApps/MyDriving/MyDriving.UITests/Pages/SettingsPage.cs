using System;
using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
    public class SettingsPage : BasePage
    {
        public SettingsPage()
            : base ("Settings", "Settings")
        {
        }

        public SettingsPage SetDistanceSetting()
        {
            app.Tap("Metric (km)");
            app.Screenshot("Set Distance Setting");

            return this;
        }

        public SettingsPage SetCapacitySetting()
        {
            app.Tap("Metric (liters)");
            app.Screenshot("Set Capacity Setting");

            return this;
        }
    }
}

