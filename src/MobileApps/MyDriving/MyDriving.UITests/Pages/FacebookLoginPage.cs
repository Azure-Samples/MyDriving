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
            App.WaitForElement(c => c.Css("INPUT._56bg._4u9z._5ruq")
                               , timeout:TimeSpan.FromSeconds(20)
                               , timeoutMessage:"Facebook login UI not displayed");
            
            App.EnterText(c => c.Css("INPUT._56bg._4u9z._5ruq"), "scott_kdnkrdr_guthrie@tfbnw.net");
            App.DismissKeyboard();
            App.EnterText(c => c.Css("#u_0_1"), "admin1");
            App.DismissKeyboard();

            App.Screenshot("Entered Facebook Credentials");

            App.Tap(c => c.Css("#u_0_5"));

            try
            {
                //some OS opt for saving passwords from webviews. This catches that case
                App.WaitForElement("Do you want the browser to remember this password?", timeout: TimeSpan.FromSeconds(5));
                App.Tap("Not now");
            }
            catch { }
                
        }
    }
}

