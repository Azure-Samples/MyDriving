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
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authentication = ServiceLocator.Instance.Resolve<IAuthentication>();
            var client = ServiceLocator.Instance.Resolve<IAzureClient>()?.Client as MobileServiceClient;
            var accountType = GetProviderType();

            if (client == null)
            {
                throw new InvalidOperationException("Make sure to set the ServiceLocator has an instance of IAzureClient");
            }

            if (authentication == null)
            {
                throw new InvalidOperationException("Make sure to set the ServiceLocator has an instance of IAuthentication");
            }

            //Clone the request in case we need to send it again
            var clonedRequest = await CloneRequest(request);
            var response = await base.SendAsync(clonedRequest, cancellationToken);

            //If the token is expired or is invalid, then we need to either refresh the token or prompt the user to log back in
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                //For MSA, attempt to refresh the token if we haven't already so that user doesn't have to log back in again
                //This isn't needed for Facebook since the token doesn't expire for 60 days and when redirecting to the login screen, Facebook automatically refreshes the token
                //Similarly, for Twitter, the token never expires
                if (accountType == MobileServiceAuthenticationProvider.MicrosoftAccount)
                {
                    if (await RefreshToken(client, cancellationToken))
                    {
                        //Resend the request now that the token has been refreshed and return the response
                        return await ResendRequest(client, request, cancellationToken);
                    }
                }

                //Otherwise if refreshing the token failed or Facebook\Twitter is being used, prompt the user to log back in via the login screen
                //First, make sure there isn't a progress dialog being displayed since this will block the main ui thread
                ProgressDialogManager.HideProgressDialog();

                var user = await authentication.LoginAsync(client, accountType);

                //Redisplay the progress dialog if needed
                ProgressDialogManager.ShowProgressDialog();

                if (user != null)
                {
                    //Resend the request since the user has successfully logged in and return the response
                    return await ResendRequest(client, request, cancellationToken);
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

        private async Task<bool> RefreshToken(IMobileServiceClient client, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> authCompletionSource = new TaskCompletionSource<bool>();

            try
            {
                //Refreshing the token could fail if this is attempted outside the timeframe of 72 hours after the access token expired
                //or if the provider invalidated the token (for example, if the user changed their password).
                HttpRequestMessage refreshTokenRequest = new HttpRequestMessage(HttpMethod.Get, client.MobileAppUri.AbsoluteUri + ".auth/refresh");
                refreshTokenRequest.Headers.Remove("X-ZUMO-AUTH");
                refreshTokenRequest.Headers.Add("X-ZUMO-AUTH", client.CurrentUser.MobileServiceAuthenticationToken);

                //Need to wait for the refresh token
                var response = base.SendAsync(refreshTokenRequest, cancellationToken).Result;

                if (response.IsSuccessStatusCode)
                {
                    string refreshContent = response.Content.ReadAsStringAsync().Result;
                    JObject refreshJson = JObject.Parse(refreshContent);
                    string newToken = refreshJson["authenticationToken"].Value<string>();
                    client.CurrentUser.MobileServiceAuthenticationToken = newToken;
                    Settings.Current.AuthToken = newToken;
                    authCompletionSource.SetResult(true);
                }
                else
                {
                    authCompletionSource.SetResult(false);
                }
            }
            catch (Exception e)
            {
                authCompletionSource.SetResult(false);
                Logger.Instance.Report(e);
            } 

            return await authCompletionSource.Task;
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
