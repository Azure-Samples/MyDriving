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
            if (OnAndroid)
            {
                new CurrentTripPage()
                    .NavigateTo("Settings");
            }
            if (OniOS)
            {
                new CurrentTripPage()
                    .NavigateTo("Profile");

                new ProfilePage()
                    .NavigateToSettings();
            }

            new SettingsPage()
                .SetDistanceSetting()
                .NavigateTo("Profile");

            new ProfilePage()
                .CheckDistanceMetric(true);
		}

		[Test]
		public void ChangeCapacityUnits ()
		{
            if (OnAndroid)
            {
                new CurrentTripPage()
                    .NavigateTo("Settings");
            }
            if (OniOS)
            {
                new CurrentTripPage()
                    .NavigateTo("Profile");

                new ProfilePage()
                    .NavigateToSettings();
            }

            new SettingsPage()
                .SetCapacitySetting()
                .NavigateTo("Profile");

            new ProfilePage()
                .CheckFuelMetric(true);
		}
	}
}