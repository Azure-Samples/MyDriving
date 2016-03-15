using System;
using System.Linq;

using NUnit.Framework;

using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace MyTrips.UITests
{
	public class LoginPage : BasePage
	{
		// Page Name
		public LoginPage ()//  : base ("", "LoginViewController")
		{
			app.Screenshot ("Social Login Page");
		}

		public void LoginWithFacebook()
		{
			app.Tap ("Login with Facebook");

			app.Screenshot ("Embedded Facebook Web View");
			app.EnterText (c => c.Css("INPUT._56bg._4u9z._5ruq"), "test_hvjvpbj_user@tfbnw.net");
			app.EnterText (c => c.Css ("#u_0_1"), "admin1");
			app.Screenshot ("Entered Facebook Credentials");
			app.Tap (c => c.Css ("#u_0_5"));

			app.WaitForElement (c => c.Marked ("Current Trip"));
			app.Screenshot ("Completed Facebook Login");
		}

		public void SkipAuthentication()
		{
			app.Tap ("Skip Auth");
		}
	}
}