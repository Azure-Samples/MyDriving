using System;
using System.Linq;

using NUnit.Framework;

using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace MyDriving.UITests
{
	public class LoginPage : BasePage
	{
        string LoginWithFacebookButton;
        string SkipAuthenticationButton = "button_skip";

		public LoginPage ()
			: base (c => c.Marked ("button_twitter"), c => c.Marked ("LoginWithTwitter"))
		{
			if (OnAndroid) {
                LoginWithFacebookButton = "button_facebook";
			} 

			if (OniOS) {
                LoginWithFacebookButton = "LoginWithFacebook";
			}
		}

		public void LoginWithFacebook()
		{
			app.Tap (LoginWithFacebookButton);

			app.Screenshot ("Embedded Facebook Web View");
			app.EnterText (c => c.Css("INPUT._56bg._4u9z._5ruq"), "scott_kdnkrdr_guthrie@tfbnw.net");
			app.EnterText (c => c.Css ("#u_0_1"), "admin1");
			app.Screenshot ("Entered Facebook Credentials");
			app.Tap (c => c.Css ("#u_0_5"));

			app.WaitForElement (c => c.Marked ("Current Trip"));
			app.Screenshot ("Facebook Authentication Succeeded");
		}

		public void SkipAuthentication()
		{
			app.Tap (SkipAuthenticationButton);
			app.Screenshot ("Authentication Skipped");
		}
	}
}