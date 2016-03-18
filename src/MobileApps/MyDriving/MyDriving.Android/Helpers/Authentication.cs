using System;
using MyDriving.Utils;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using MyDriving.Interfaces;
using Plugin.CurrentActivity;

namespace MyDriving.Droid.Helpers
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
                Settings.Current.AzureMobileUserId = user?.UserId ?? string.Empty;
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

