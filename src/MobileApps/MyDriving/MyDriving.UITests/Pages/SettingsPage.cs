using System;
using Xamarin.UITest;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
    public class SettingsPage : BasePage
    {
        readonly Func<string, Query> CheckBox;

        public SettingsPage()
            : base ("Settings", "Settings")
        {
            if (OniOS)
            { 
            }
            if (OnAndroid)
            { 
                CheckBox = (arg) => x => x.Marked(arg).Parent(0).Sibling().Descendant().Id("checkbox"); 
            }
        }

        public SettingsPage SetDistanceSetting()
        {
            app.Tap(CheckBox("Metric Distance"));
            app.Screenshot("Using Metric Distances");

            return this;
        }

        public SettingsPage SetCapacitySetting()
        {
            app.Tap(CheckBox("Metric Units"));
            app.Screenshot("Using Metric Capacity");

            return this;
        }
    }
}

