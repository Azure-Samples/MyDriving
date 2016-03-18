using System;
using NUnit.Framework;
using Xamarin.UITest;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
	public class ProfilePage : BasePage
	{
        readonly Query FuelConsumptionField;
        readonly Query DistanceField;

		public ProfilePage ()
            : base ("profile_image", "ios_trait")
		{
            if (OnAndroid)
            { 
                FuelConsumptionField = x => x.Id("text_fuel_consumption");
                DistanceField = x => x.Id("text_distance");
            }
            if (OniOS)
            { 
            }
		}

		public ProfilePage SetDeviceConnectionSetting ()
		{
			app.EnterText(x => x.Class("UITextField"), "http://build2016.azurewebsites.net");
			app.Screenshot ("Set Device Connection String Setting");

			return this;
		}

		public ProfilePage NavigateToMobileClientUrlSetting ()
		{
			app.Tap ("Mobile client URL");

			return this;
		}

		public ProfilePage SetMobileClientUrlSetting ()
		{
			app.EnterText(x => x.Class("UITextField"), "http://build2016.azurewebsites.net");
			app.Screenshot ("Set Mobile Client Url Setting");

			return this;
		}
			
		public ProfilePage Logout ()
		{
			app.ScrollDownTo("Log Out");
			app.Tap(x => x.Marked("Log Out"));
			app.Tap(x => x.Marked("Yes, Logout"));

			return this;
		}

        public ProfilePage CheckFuelMetric(bool metric)
        {
            app.ScrollDownTo(FuelConsumptionField);
            var fuelText = app.Query(FuelConsumptionField)[0].Text;
            app.Screenshot("Verifying fuel units correct");

            var expectedUnits = metric ? "l" : "gal";
            StringAssert.Contains(expectedUnits, fuelText, message:"Couldnt verify fuel units");

            return this;
        }

        public ProfilePage CheckDistanceMetric(bool metric)
        {
            app.ScrollDownTo(DistanceField);
            var distanceText = app.Query(DistanceField)[0].Text;
            app.Screenshot("Verifying distance units correct");

            var expectedUnits = metric ? "km" : "miles";
            StringAssert.Contains(expectedUnits, distanceText, message: "Couldnt verify distance units");

            return this;
        }
	}
}