using System;
using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
    public class SettingsPage : BasePage
    {
        readonly Func<string, Query> CheckBox;
        readonly Query SettingsTab;

        public SettingsPage()
            : base ("Settings", "Settings")
        {
            if (OniOS)
            {
                SettingsTab = x => x.Class("UINavigationItemButtonView").Marked("Settings");
            }
            if (OnAndroid)
            { 
                CheckBox = (arg) => x => x.Marked(arg).Parent(0).Sibling().Descendant().Id("checkbox"); 
            }
        }

        public SettingsPage SetDistanceSetting()
        {
            if (OnAndroid)
            {
                App.Tap(CheckBox("Metric Distance"));
                App.Screenshot("Using Metric Distances");
            }
            if (OniOS)
            { 
                App.Tap("Distance");
                App.Tap("Metric (km)");
                App.Screenshot("Using Metric Distances");

                App.Tap(SettingsTab);
            }

            return this;
        }

        public SettingsPage SetCapacitySetting()
        {
            if (OnAndroid)
            {
                App.Tap(CheckBox("Metric Units"));
                App.Screenshot("Using Metric Capacity");
            }
            if (OniOS)
            {
                App.Tap("Capacity");
                App.Tap("Metric (liters)");
                App.Screenshot("Using Metric Capacity");

                App.Tap(SettingsTab);
            }

            return this;
        }

        new public void NavigateTo(string marked)
        {
            if (OnAndroid)
                base.NavigateTo(marked);
            
            if (OniOS)
                App.Tap("Back");
        }
    }
}

