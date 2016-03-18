using System;
namespace MyDriving.UITests
{
    public class FacebookLoginPage : BasePage
    {
        public FacebookLoginPage()
            :base(x => x.WebView(), x => x.WebView())
        {
        }

        public void Login()
        {
            app.EnterText(c => c.Css("INPUT._56bg._4u9z._5ruq"), "scott_kdnkrdr_guthrie@tfbnw.net");
            app.EnterText(c => c.Css("#u_0_1"), "admin1");

            app.Screenshot("Entered Facebook Credentials");

            app.Tap(c => c.Css("#u_0_5"));
        }
    }
}

