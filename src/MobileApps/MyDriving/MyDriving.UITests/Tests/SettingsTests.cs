using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyDriving.UITests
{
	public class SettingsTests : AbstractSetup
	{
		public SettingsTests (Platform platform) : base (platform)
		{
		}

		[Test]
		public void ChangeDistanceUnits ()
		{
            new CurrentTripPage()
                .NavigateTo("Settings");

            new SettingsPage()
                .SetDistanceSetting()
                .NavigateTo("Profile");

			//new ProfilePage ()
			//	.NavigateToProfilePage ()
			//	.NavigateToSettingsPage ()
			//	.NavigateToDistanceSetting ()
			//	.SetDistanceSetting ()
			//	.NavigateFromSettingsDetailPage ();

			app.Screenshot ("Changed Distance Setting");
		}

		[Test]
		public void ChangeCapacityUnits ()
		{
            new CurrentTripPage()
                .NavigateTo("Settings");

            new SettingsPage()
                .SetCapacitySetting()
                .NavigateTo("Profile");

			//new ProfilePage ()
			//	.NavigateToProfilePage ()
			//	.NavigateToSettingsPage ()
			//	.NavigateToCapacitySetting ()
			//	.SetCapacitySetting ()
			//	.NavigateFromSettingsDetailPage ();

			app.Screenshot ("Changed Capacity Setting");
		}

		

		[Test]
		public void Logout ()
		{
            new CurrentTripPage()
                .NavigateTo("Settings");

			//new ProfilePage ()
			//	.NavigateToProfilePage ()
			//	.NavigateToSettingsPage ()
			//	.Logout ();

			app.Screenshot ("Logged out");
		}
	}
}