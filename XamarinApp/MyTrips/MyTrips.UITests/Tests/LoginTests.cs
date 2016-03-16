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
		public void SkipAuthenticationTest()
		{
			ClearKeychain ();

			new LoginPage ()
				.SkipAuthentication ();
		}

		[Test]
		public void LoginWithFacebookTest()
		{
			ClearKeychain ();

			new LoginPage ()
				.LoginWithFacebook ();
		}
	}
}