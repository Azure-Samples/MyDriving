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

		public void NavigateToSettingsPage ()
		{
			app.Tap (c => c.Class ("UIImageView").Marked ("tab_Settings.png"));
		}
	}
}