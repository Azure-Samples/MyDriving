using System;
using System.Linq;

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
            app.WaitForElement(c => c.Css("INPUT._56bg._4u9z._5ruq")
                               , timeout:TimeSpan.FromSeconds(20)
                               , timeoutMessage:"Facebook login UI not displayed");
            
            app.EnterText(c => c.Css("INPUT._56bg._4u9z._5ruq"), "scott_kdnkrdr_guthrie@tfbnw.net");
            app.DismissKeyboard();
            app.EnterText(c => c.Css("#u_0_1"), "admin1");
            app.DismissKeyboard();

            app.Screenshot("Entered Facebook Credentials");

            app.Tap(c => c.Css("#u_0_5"));

            try
            {
                //some OS opt for saving passwords from webviews. This catches that case
                app.WaitForElement("Do you want the browser to remember this password?", timeout: TimeSpan.FromSeconds(5));
                app.Tap("Not now");
            }
            catch { }
                
        }
    }
}

