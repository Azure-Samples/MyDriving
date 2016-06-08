// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

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
			ToProfilePage().CheckDistanceMetric(true);
		}

		[Test]
		public void ChangeCapacityUnits ()
		{
			ToProfilePage().CheckFuelMetric(true);
		}

		public ProfilePage ToProfilePage()
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

			return new ProfilePage ();
		}
	}
}