// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using MyDriving.Utils;
using MyDriving.Utils.Interfaces;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Plugin.CurrentActivity;

namespace MyDriving.Droid.Helpers
{
    public class Authentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client,
            MobileServiceAuthenticationProvider provider)
        {
            try
            {
                Settings.Current.LoginAttempts++;
                var user = await client.LoginAsync(CrossCurrentActivity.Current.Activity, provider);
                Settings.Current.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                Settings.Current.AzureMobileUserId = user?.UserId ?? string.Empty;
                return user;
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("cancelled"))
                {
                    e.Data["method"] = "LoginAsync";
                    Logger.Instance.Report(e);
                }
            }

            return null;
        }

        public void ClearCookies()
        {
            try
            {
                if ((int) Android.OS.Build.VERSION.SdkInt >= 21)
                    Android.Webkit.CookieManager.Instance.RemoveAllCookies(null);
            }
            catch (Exception ex)
            {
                Logger.Instance.Report(ex);
            }
        }
    }
}