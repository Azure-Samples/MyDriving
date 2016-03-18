using System;
using Xamarin.UITest;

namespace MyDriving.UITests
{
	public class ProfilePage : BasePage
	{
		public ProfilePage ()
            : base ("profile_image", "ios_trait")
		{
		}

		//public ProfilePage NavigateToProfilePage ()
		//{
		//	app.Tap ("Profile");

		//	return this;
		//}

		//public ProfilePage NavigateToSettingsPage ()
		//{
  //          NavigateTo("Settings");
		//	return this;
		//}

		//public ProfilePage NavigateFromSettingsDetailPage ()
		//{
		//	app.Tap ("Settings");

		//	return this;
		//}

		public ProfilePage NavigateToDistanceSetting ()
		{
			app.Tap ("Distance");

			return this;
		}

		public ProfilePage NavigateToCapacitySetting ()
		{
			app.Tap ("Capacity");

			return this;
		}

		public ProfilePage NavigateToDeviceConnectionSetting ()
		{
			app.Tap ("Device connection string");

			return this;
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
	}
}