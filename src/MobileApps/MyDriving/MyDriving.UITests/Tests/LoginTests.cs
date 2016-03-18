using System;

using NUnit.Framework;
using Xamarin.UITest;

namespace MyDriving.UITests
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

            new CurrentTripPage()
                .AssertOnPage();
		}

		[Test]
		public void LoginWithFacebookTest()
		{
			ClearKeychain ();

			new LoginPage ()
				.LoginWithFacebook ();

            new FacebookLoginPage()
                .Login();

            new CurrentTripPage()
                .AssertOnPage();
		}
	}
}