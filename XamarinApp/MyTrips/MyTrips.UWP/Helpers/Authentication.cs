using Microsoft.WindowsAzure.MobileServices;
using MyTrips.Utils;
using MyTrips.Interfaces;
using System;
using System.Threading.Tasks;

namespace MyTrips.UWP.Helpers
{
    public class Authentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var user = await client.LoginAsync(provider);

                Settings.Current.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                Settings.Current.UserId = user?.UserId ?? string.Empty;

                return user;
            }
            catch (Exception e)
            {
                e.Data["method"] = "LoginAsync";
                Logger.Instance.Report(e);
            }

            return null;
        }

        public void ClearCookies()
        {

        }
    }
}
