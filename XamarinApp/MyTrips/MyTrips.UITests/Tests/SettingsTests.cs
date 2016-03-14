using System;
using Xamarin.UITest;
using NUnit.Framework;

namespace MyTrips.UITests
{
	public class SettingsTests : AbstractSetup
	{
		public SettingsTests (Platform platform) : base (platform)
		{
		}

		public void ChangeDistanceUnits ()
		{
			new ProfilePage ()
				.NavigateToProfilePage ()
				.NavigateToSettingsPage ();
		}

		public void ChangeCapacityUnits ()
		{

		}

		public void ChangeDeviceConnectionString ()
		{

		}

		public void ChangeMobileServiceUrl ()
		{

		}

		public void Logout ()
		{

		}
	}
}