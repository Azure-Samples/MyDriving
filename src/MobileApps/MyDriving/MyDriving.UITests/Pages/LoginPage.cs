// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

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
			App.Tap (LoginWithFacebookButton);
            App.Screenshot("Selecting Facebook Login");
		}

		public void SkipAuthentication()
		{
			App.Tap (SkipAuthenticationButton);
			App.Screenshot ("Authentication Skipped");
		}
	}
}