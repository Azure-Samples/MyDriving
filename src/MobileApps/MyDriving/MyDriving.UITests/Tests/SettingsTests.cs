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

            new ProfilePage()
                .CheckDistanceMetric(true);
		}

		[Test]
		public void ChangeCapacityUnits ()
		{
            new CurrentTripPage()
                .NavigateTo("Settings");

            new SettingsPage()
                .SetCapacitySetting()
                .NavigateTo("Profile");

            new ProfilePage()
                .CheckFuelMetric(true);
		}

		[Test]
		public void Logout ()
		{
            ClearKeychain();

            new LoginPage()
                .LoginWithFacebook();

            new FacebookLoginPage()
                .Login();

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