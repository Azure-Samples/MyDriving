using System;

using NUnit.Framework;
using Xamarin.UITest;

namespace MyTrips.UITests
{
	// [TestFixture (Platform.iOS)]
	public class LoginTests : AbstractSetup
	{
		public LoginTests (Platform platform) : base (platform)
		{
		}

		[Test]
		public void AppLaunchesSuccessfully()
		{
			app.Screenshot("App Launch");
		}

		[Test]
		public void SkipAuthentication()
		{
			ClearKeychain ();

			new LoginPage ()
				.SkipAuthentication ();

			app.Screenshot ("Authentication Skipped");
		}

		[Test]
		public void LoginWithFacebook()
		{
			ClearKeychain ();

			new LoginPage ()
				.LoginWithFacebook ();

			app.Screenshot ("Facebook Authentication Succeeded");
		}
	}
}