using System;
using MyTrips.Utils;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using MyTrips.Interfaces;
using Plugin.CurrentActivity;

namespace MyTrips.Droid.Helpers
{
    public class Authentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {

                Settings.Current.LoginAttempts++;
                var user = await client.LoginAsync(CrossCurrentActivity.Current.Activity, provider);
                Settings.Current.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                Settings.Current.UserId = user?.UserId ?? string.Empty;
                return user;
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
            try
            {
                if((int)global::Android.OS.Build.VERSION.SdkInt >= 21)
                    global::Android.Webkit.CookieManager.Instance.RemoveAllCookies(null);
            }
            catch(Exception ex)
            {
                Logger.Instance.Report(ex);
            }
        }
    }
}

