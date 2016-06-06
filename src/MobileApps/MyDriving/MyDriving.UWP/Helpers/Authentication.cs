// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using MyDriving.Utils;
using MyDriving.Utils.Interfaces;

namespace MyDriving.UWP.Helpers
{
    public class Authentication : IAuthentication
    {
        public void ClearCookies()
        {
        }

        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client,
            MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var user = await client.LoginAsync(provider);

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

        Task<MobileServiceUser> IAuthentication.LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            //TODO: Need to implement this so that it switches to main ui thread appropriately
            throw new NotImplementedException();
        }
    }
}