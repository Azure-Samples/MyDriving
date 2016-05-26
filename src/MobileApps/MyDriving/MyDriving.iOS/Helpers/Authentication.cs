// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Threading.Tasks;
using Foundation;
using MyDriving.Utils;
using Microsoft.WindowsAzure.MobileServices;
using MyDriving.Utils.Interfaces;
using System.Threading;
using MyDriving.AzureClient;

namespace MyDriving.iOS.Helpers
{
    public class Authentication : IAuthentication
    {

        public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            MobileServiceUser user = null;

            //TODO: clean up this code so not redundant
            if (Thread.CurrentThread.IsBackground)
            {
                //If this is currently executed in a background thread, switch to the main ui thread in order to get the current
                //view's context to in order to show the log in screen
                new NSObject().InvokeOnMainThread(async () =>
                {
                    try
                    {
                        var window = UIKit.UIApplication.SharedApplication.KeyWindow;
                        var root = window.RootViewController;
                        if (root != null)
                        {
                            var current = root;
                            while (current.PresentedViewController != null)
                            {
                                current = current.PresentedViewController;
                            }

                            Settings.Current.LoginAttempts++;

                            user = await client.LoginAsync(current, provider);
                        }
                    }
                    catch (Exception e)
                    {
                        if (e.Message.Contains("Authentication was cancelled"))
                        {
                            AuthenticationManager.IsAuthenticating = false;
                            e.Data["method"] = "LoginAsync";
                            Logger.Instance.Report(e);
                        }
                    }
                });
            }
            else
            {
                try
                {
                    var window = UIKit.UIApplication.SharedApplication.KeyWindow;
                    var root = window.RootViewController;
                    if (root != null)
                    {
                        var current = root;
                        while (current.PresentedViewController != null)
                        {
                            current = current.PresentedViewController;
                        }

                        Settings.Current.LoginAttempts++;

                        user = await client.LoginAsync(current, provider);
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Authentication was cancelled"))
                    {
                        AuthenticationManager.IsAuthenticating = false;
                        e.Data["method"] = "LoginAsync";
                        Logger.Instance.Report(e);
                    }
                }
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

        //async Task MyTest(IMobileServiceClient client, MobileServiceAuthenticationProvider provider)
        //{
        //        try
        //        {
        //            var window = UIKit.UIApplication.SharedApplication.KeyWindow;
        //            var root = window.RootViewController;
        //            if (root != null)
        //            {
        //                var current = root;
        //                while (current.PresentedViewController != null)
        //                {
        //                    current = current.PresentedViewController;
        //                }

        //                Settings.Current.LoginAttempts++;

        //                var user = await client.LoginAsync(current, provider);
        //                string s = user.MobileServiceAuthenticationToken;
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            if (!e.Message.Contains("cancelled"))
        //            {
        //                e.Data["method"] = "LoginAsync";
        //                Logger.Instance.Report(e);
        //            }
        //        }
        //    }

        //public async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client,
        //    MobileServiceAuthenticationProvider provider)
        //{

        //    MobileServiceUser user = null;

        //    await Task.Run(() => new NSObject().InvokeOnMainThread(async delegate { await Test(client, provider); }));
        //    //{
        //        //try
        //        //{
        //        //    var window = UIKit.UIApplication.SharedApplication.KeyWindow;
        //        //    var root = window.RootViewController;
        //        //    if (root != null)
        //        //    {
        //        //        var current = root;
        //        //        while (current.PresentedViewController != null)
        //        //        {
        //        //            current = current.PresentedViewController;
        //        //        }

        //        //        Settings.Current.LoginAttempts++;

        //        //        user = await client.LoginAsync(current, provider);
        //        //    }
        //        //}
        //        //catch (Exception e)
        //        //{
        //        //    if (!e.Message.Contains("cancelled"))
        //        //    {
        //        //        e.Data["method"] = "LoginAsync";
        //        //        Logger.Instance.Report(e);
        //        //    }
        //        //}
        //    //});

        //    Settings.Current.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
        //    Settings.Current.AzureMobileUserId = user?.UserId ?? string.Empty;

        //    return user;
        //}

        //private async Task<MobileServiceUser> Test(IMobileServiceClient client,
        //    MobileServiceAuthenticationProvider provider)
        //{
        //    MobileServiceUser user = null;

        //    try
        //    {
        //        var window = UIKit.UIApplication.SharedApplication.KeyWindow;
        //        var root = window.RootViewController;
        //        if (root != null)
        //        {
        //            var current = root;
        //            while (current.PresentedViewController != null)
        //            {
        //                current = current.PresentedViewController;
        //            }

        //            Settings.Current.LoginAttempts++;

        //            ////Use a lock to prevent the log-in screen from being displayed more than once
        //            //lock (AuthenticationManager.AuthLock)
        //            //{
        //            //    if (AuthenticationManager.IsLoggingIn)
        //            //    {
        //            //        return user;
        //            //    }
        //            //    else
        //            //    {
        //            //        AuthenticationManager.IsLoggingIn = true;
        //            //    }
        //            //}

        //            user = await client.LoginAsync(current, provider);

        //            //lock (AuthenticationManager.AuthLock)
        //            //{
        //            //    AuthenticationManager.IsLoggingIn = false;
        //            //}

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (!e.Message.Contains("cancelled"))
        //        {
        //            e.Data["method"] = "LoginAsync";
        //            Logger.Instance.Report(e);
        //        }
        //    }

        //    return user;
        //}

    }
}