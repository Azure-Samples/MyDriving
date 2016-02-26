using System;
using System.Threading.Tasks;

using Foundation;

using MyTrips.Interfaces;
using MyTrips.Utils;

using Microsoft.WindowsAzure.MobileServices;

namespace MyTrips.iOS.Helpers
{
    public class Authentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var window = UIKit.UIApplication.SharedApplication.KeyWindow;
                var root = window.RootViewController;
                if(root != null)
                {
                    var current = root;
                    while(current.PresentedViewController != null)
                    {
                        current = current.PresentedViewController;
                    }


                    Settings.Current.LoginAttempts++;

                    var user = await client.LoginAsync(current, provider);

                    Settings.Current.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                    Settings.Current.UserId = user?.UserId ?? string.Empty;

                    return user;
                }
            }
            catch(Exception e)
            {
                e.Data["method"] = "LoginAsync";
                Logger.Instance.Report(e);
            }

            return null;
        }

        public void ClearCookies()
        {
            var store = NSHttpCookieStorage.SharedStorage;
            var cookies = store.Cookies;

            foreach(var c in cookies)
            {
                store.DeleteCookie(c);
            }
        }
    }
}