using System;

using NUnit.Framework;
using Xamarin.UITest;

namespace MyTrips.UITests
{
	public class LoginTests : AbstractSetup
	{
		public LoginTests (Platform platform) : base (platform)
		{
		}

		[Test]
		public void AppLaunchesSuccessfullyTest()
		{
			app.Screenshot("App Launch");
		}

		[Test]
		public void SkipAuthenticationTest()
		{
			ClearKeychain ();

			new LoginPage ()
				.SkipAuthentication ();

			app.Screenshot ("Authentication Skipped");
		}

		[Test]
		public void LoginWithFacebookTest()
		{
			ClearKeychain ();

			new LoginPage ()
				.LoginWithFacebook ();

			app.Screenshot ("Facebook Authentication Succeeded");
		}
	}
}