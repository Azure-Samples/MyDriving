// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using MyDriving.Utils;
using MyDriving.Utils.Interfaces;
using Windows.UI.Core;

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
            var coreWindow = Windows.ApplicationModel.Core.CoreApplication.MainView;
            // Dispatcher needed to run on UI Thread
            var dispatcher = coreWindow.CoreWindow.Dispatcher;

            MobileServiceUser user = null;


            try
            {
                if (dispatcher.HasThreadAccess)  //is running on UI thread
                {
                    user = await client.LoginAsync(provider);
                }
                else
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        user = await client.LoginAsync(provider);

                    });
                }

                if (user != null)
                {
                    Settings.Current.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                    Settings.Current.AzureMobileUserId = user?.UserId ?? string.Empty;
                }

            }
            catch (Exception e)
            {
                if (!e.Message.Contains("cancelled"))
                {
                    e.Data["method"] = "LoginAsync";
                    Logger.Instance.Report(e);
                }
            }

            return user;
        }

    }
}