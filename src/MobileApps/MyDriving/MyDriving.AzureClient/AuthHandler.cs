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

            //Send the request and check to see what the response is
            var response = await base.SendAsync(request, cancellationToken);

            //If this is a refresh token request that failed, then the user needs to be prompted to log back in
            if (request.RequestUri.AbsoluteUri.Contains("/.auth/refresh") && response.StatusCode != HttpStatusCode.OK)
            {
                //If there is a progress dialog open, hide it so that the UI isn't blocked 
                ProgressDialogManager.HideProgressDialog();

                //Prompt the user to log back in
                MobileServiceUser user = await authentication.LoginAsync(client, accountType);

                //Attempt to resend the request; however, if the user is still unauthenticated, then the original response is returned
                if (user != null)
                {
                    //If there was a progress dialog previously open, show it again and resend the request
                    ProgressDialogManager.ShowProgressDialog();
                    response = await ResendRequest(client, request, cancellationToken);
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // The user is not logged in - we got a 401 (likely the token expired, but this can occur if the token was revoked for some other reason)
                // 1.) If MSA is being used, this access token expires every hour; when this happens, attempt to get a refresh token so that the user 
                //     doesn't need to re-login.
                // 2.) If Facebook is being used, the access token expires every 60 days; since this is such a long period of time, simply prompt the user
                //     to log back in when authentication fails.
                // 3.) If Twitter is used, the access token never expires; however, the access token can still be revoked for other reasons, so simply prompt the user
                //     to log back if authentication fails.
                try
                {
                    MobileServiceUser user = null;

                    if (accountType == MobileServiceAuthenticationProvider.MicrosoftAccount) 
                    {
                        user = await RefreshToken(client);
                    }
                    else
                    {
                        //If there is a progress dialog open, hide it so that the UI isn't blocked 
                        ProgressDialogManager.HideProgressDialog();

                        //Prompt the user to log back in
                        user = await authentication.LoginAsync(client, accountType);
                    }

                    //Attempt to resend the request; however, if the user is still unauthenticated, then the original response is returned
                    if (user != null)
                    {
                        //If there was a progress dialog previously open, show it again
                        ProgressDialogManager.ShowProgressDialog();
                        response = await ResendRequest(client, request, cancellationToken);
                    }
                }
                catch
                {
                    // Exception occurred, so return original response
                    return response;
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

        private async Task<MobileServiceUser> RefreshToken(IMobileServiceClient client)
        {
            MobileServiceUser user = null;
            JObject refreshJson = (JObject)await client.InvokeApiAsync("/.auth/refresh", HttpMethod.Get, null);

            if (refreshJson != null)
            {
                string newToken = refreshJson["authenticationToken"].Value<string>();
                client.CurrentUser.MobileServiceAuthenticationToken = newToken;
                user = client.CurrentUser;
            }

            return user;
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
