using System;
using Xamarin.UITest;

namespace MyTrips.UITests
{
	public class ProfilePage : BasePage
	{
		public ProfilePage ()
		{
			app.Screenshot ("Profile Page");
		}

		public ProfilePage NavigateToProfilePage ()
		{
			app.Tap ("Profile");

			return this;
		}

		public ProfilePage NavigateToSettingsPage ()
		{
			app.Tap (c => c.Class ("UIImageView").Marked ("tab_Settings.png"));
			app.Screenshot ("Settings Page");

			return this;
		}

		public ProfilePage NavigateToCapacitySetting ()
		{
			// TODO 
			return this;
		}

		public ProfilePage NavigateToDistanceSetting ()
		{
			// TODO 
			return this;
		}
	}
}