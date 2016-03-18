// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace MyDriving.UITests
{
    public class LoginPage : BasePage
    {
        string LoginWithFacebookItem = "LoginWithFacebook";
        string SkipAuthenticationItem = "SkipAuthentication";

        public LoginPage()
            : base(c => c.Marked("LoginWithTwitter"), c => c.Marked("LoginWithTwitter"))
        {
            if (OnAndroid)
            {
            }

            if (OniOS)
            {
            }
        }

        public void LoginWithFacebook()
        {
            App.Tap(LoginWithFacebookItem);

            App.Screenshot("Embedded Facebook Web View");
            App.EnterText(c => c.Css("INPUT._56bg._4u9z._5ruq"), "scott_kdnkrdr_guthrie@tfbnw.net");
            App.EnterText(c => c.Css("#u_0_1"), "admin1");
            App.Screenshot("Entered Facebook Credentials");
            App.Tap(c => c.Css("#u_0_5"));

            App.WaitForElement(c => c.Marked("Current Trip"));
            App.Screenshot("Facebook Authentication Succeeded");
        }

        public void SkipAuthentication()
        {
            App.Tap(SkipAuthenticationItem);
            App.Screenshot("Authentication Skipped");
        }
    }
}