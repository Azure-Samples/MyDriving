using System;
using System.Linq;

using NUnit.Framework;

using Xamarin.UITest;
using Xamarin.UITest.Queries;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace MyDriving.UITests
{
	public class LoginPage : BasePage
	{
        string LoginWithFacebookButton;
        string SkipAuthenticationButton;

		public LoginPage ()
			: base (c => c.Marked ("button_twitter"), c => c.Marked ("LoginWithTwitter"))
		{
			if (OnAndroid) {
                LoginWithFacebookButton = "button_facebook";
                SkipAuthenticationButton = "button_skip";
			} 

			if (OniOS) {
                LoginWithFacebookButton = "LoginWithFacebook";
                SkipAuthenticationButton = "Skip Auth";
			}
		}

		public void LoginWithFacebook()
		{
			app.Tap (LoginWithFacebookButton);
            app.Screenshot("Selecting Facebook Login");
		}

		public void SkipAuthentication()
		{
			app.Tap (SkipAuthenticationButton);
			app.Screenshot ("Authentication Skipped");
		}
	}
}