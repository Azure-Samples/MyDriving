// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Threading.Tasks;
using Foundation;
using MyDriving.Utils;
using Microsoft.WindowsAzure.MobileServices;
using MyDriving.Utils.Interfaces;
using System.Threading;
namespace MyDriving.iOS.Helpers
{
    public class Authentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            TaskCompletionSource<MobileServiceUser> authCompletionSource = new TaskCompletionSource<MobileServiceUser>();

            MobileServiceUser user = null;

            //TODO: Likely don't need this code to run in background if we remove ConfigureAwait(false)
            //if (Thread.CurrentThread.IsBackground)
            //{
            //    //The log-in screen cannot be redisplayed unless on the main UI thread
            //    new NSObject().InvokeOnMainThread((async () =>
            //    {
            //        user = await LoginAsyncHelper(client, provider);
            //        authCompletionSource.SetResult(user);
            //    }));
            //}
            //else
            //{
                user = await LoginAsyncHelper(client, provider);
                authCompletionSource.SetResult(user);
            //}

            return await authCompletionSource.Task;
        }

        private async Task<MobileServiceUser> LoginAsyncHelper(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            MobileServiceUser user = null;

            try
            {
                var window = UIKit.UIApplication.SharedApplication.KeyWindow;
                var current = window.RootViewController;
                while (current.PresentedViewController != null)
                {
                    current = current.PresentedViewController;
                }

                Settings.Current.LoginAttempts++;

                user = await client.LoginAsync(current, provider);
            }
            catch (Exception e)
            {
                //InvalidOperationException thrown if the user chooses to cancel out of the log in screen
                //Only log exception when other exception types are thrown
                if (!(e is InvalidOperationException) && e.Message.Contains("cancelled"))
                {
                    e.Data["method"] = "LoginAsyncHelper";
                    Logger.Instance.Report(e);
                }
            }
            finally
            {
                //If the user failed to authenticate, set the auth token and user id to empty string
                Settings.Current.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                Settings.Current.AzureMobileUserId = user?.UserId ?? string.Empty;
            }

            return user;
        }

        public void ClearCookies()
        {
            var store = NSHttpCookieStorage.SharedStorage;
            var cookies = store.Cookies;

            foreach (var c in cookies)
            {
                store.DeleteCookie(c);
            }
        }
    }
}