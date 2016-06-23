// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading;
using MyDriving.Utils;
using System.Text;
using MyDriving.Utils.Interfaces;
using MyDriving.Utils.Helpers;
using Newtonsoft.Json.Linq;

namespace MyDriving.AzureClient
{
    class AuthHandler : DelegatingHandler
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private static bool isReauthenticating = false;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //Clone the request in case we need to send it again
            var clonedRequest = await CloneRequest(request);
            var response = await base.SendAsync(clonedRequest, cancellationToken);

            //If the token is expired or is invalid, then we need to either refresh the token or prompt the user to log back in
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (isReauthenticating)
                    return response;

                var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client as MobileServiceClient;
                if (client == null)
                {
                    throw new InvalidOperationException(
                        "Make sure to set the ServiceLocator has an instance of IAzureClient");
                }

                string authToken = client.CurrentUser.MobileServiceAuthenticationToken;
                await semaphore.WaitAsync();
                //In case two threads enter this method at the same time, only one should do the refresh (or re-login), the other should just resend the request with an updated header.
                if (authToken != client.CurrentUser.MobileServiceAuthenticationToken)  // token was already renewed
                {
                    semaphore.Release();
                    return await ResendRequest(client, request, cancellationToken);
                }

                isReauthenticating = true;
                bool gotNewToken = false;
                try
                {
                    //For MSA, attempt to refresh the token if we haven't already so that user doesn't have to log back in again
                    //This isn't needed for Facebook since the token doesn't expire for 60 days; similarly, for Twitter, the token never expires
                    if (Settings.Current.LoginAccount == LoginAccount.Microsoft)
                    {
                        gotNewToken = await RefreshToken(client);
                    }

                    //Otherwise if refreshing the token failed or Facebook\Twitter is being used, prompt the user to log back in via the login screen
                    if (!gotNewToken)
                    {
                        //First, make sure there isn't a progress dialog being displayed since this will block the main ui thread
                        ProgressDialogManager.HideProgressDialog();

                        gotNewToken = await Login(client);

                        //Redisplay the progress dialog if needed
                        ProgressDialogManager.ShowProgressDialog();
                    }
                }
                catch (System.Exception e)
                {
                    Logger.Instance.Report(e);
                }
                finally
                {
                    isReauthenticating = false;
                    semaphore.Release();
                }


                if (gotNewToken)
                {
                    if (!request.RequestUri.OriginalString.Contains("/.auth/me"))   //do not resend in this case since we're not using the return value of auth/me
                    {
                        //Resend the request since the user has successfully logged in and return the response
                        return await ResendRequest(client, request, cancellationToken);
                    }
                }
            }

            return response;
        }

        private MobileServiceAuthenticationProvider GetProviderType()
        {
            var accountType = MobileServiceAuthenticationProvider.MicrosoftAccount;
            switch (Settings.Current.LoginAccount)
            {
                case LoginAccount.Facebook:
                    accountType = MobileServiceAuthenticationProvider.Facebook;
                    break;
                case LoginAccount.Twitter:
                    accountType = MobileServiceAuthenticationProvider.Twitter;
                    break;
            }

            return accountType;
        }

        private async Task<HttpResponseMessage> ResendRequest(IMobileServiceClient client, HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Clone the request
            var clonedRequest = await CloneRequest(request);

            // Set the authentication header
            clonedRequest.Headers.Remove("X-ZUMO-AUTH");
            clonedRequest.Headers.Add("X-ZUMO-AUTH", client.CurrentUser.MobileServiceAuthenticationToken);

            // Resend the request
            return await base.SendAsync(clonedRequest, cancellationToken);
        }


        private async Task<bool> RefreshToken(IMobileServiceClient client)
        {
            try
            {
                JObject refreshJson = (JObject)await client.InvokeApiAsync("/.auth/refresh", HttpMethod.Get, null);

                if (refreshJson != null)
                {
                    string newToken = refreshJson["authenticationToken"].Value<string>();
                    client.CurrentUser.MobileServiceAuthenticationToken = newToken;
                    Settings.Current.AuthToken = newToken;
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Logger.Instance.Report(e);
            }

            return false;
        }

        private async Task<bool> Login(IMobileServiceClient client)
        {
            var authentication = ServiceLocator.Instance.Resolve<IAuthentication>();
            if (authentication == null)
            {
                throw new InvalidOperationException("Make sure the ServiceLocator has an instance of IAuthentication");
            }


            var accountType = GetProviderType();
            try
            {
                var user = await authentication.LoginAsync(client, accountType);
                if (user != null)
                    return true;
            }
            catch (System.Exception e)
            {
                Logger.Instance.Report(e);
            }

            return false;
        }

        private async Task<HttpRequestMessage> CloneRequest(HttpRequestMessage request)
        {
            var result = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            if (request.Content != null && request.Content.Headers.ContentType != null)
            {
                var requestBody = await request.Content.ReadAsStringAsync();
                var mediaType = request.Content.Headers.ContentType.MediaType;
                result.Content = new StringContent(requestBody, Encoding.UTF8, mediaType);
                foreach (var header in request.Content.Headers)
                {
                    if (!header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            return result;
        }
    }
}
